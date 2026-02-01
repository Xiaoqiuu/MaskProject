import cv2
import mediapipe as mp
import math
import socket
import time
import json

# ================= Configuration =================
# UDP Configuration
UDP_IP = "127.0.0.1"  # Unity runs on the same machine
UDP_PORT = 5052       # Port to listen on in Unity
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

# Hand Tracking Configuration
# model_complexity=0 is the fastest (Lite model)
# max_num_hands=1 restricts detection to one hand for speed
mp_hands = mp.solutions.hands
hands = mp_hands.Hands(
    static_image_mode=False,
    max_num_hands=1,
    model_complexity=0, 
    min_detection_confidence=0.5,
    min_tracking_confidence=0.5
)
mp_draw = mp.solutions.drawing_utils

# Pinch Threshold (0.05 is roughly 5% of screen dimension)
PINCH_THRESHOLD = 0.05 

# Camera Setup
cap = cv2.VideoCapture(0)
cap.set(cv2.CAP_PROP_FPS, 60) # Try to request 60 FPS
cap.set(cv2.CAP_PROP_FRAME_WIDTH, 640) # Lower resolution for speed
cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 480)

print(f"Start sending data to {UDP_IP}:{UDP_PORT}...")
print("Press 'q' to quit.")

prev_time = 0

try:
    while True:
        success, img = cap.read()
        if not success:
            continue

        # Convert to RGB for MediaPipe
        img_rgb = cv2.cvtColor(img, cv2.COLOR_BGR2RGB)
        
        # Process Hand Tracking
        results = hands.process(img_rgb)
        
        data_to_send = {
            "is_pinched": False,
            "distance": 0.0,
            "hand_detected": False
        }

        if results.multi_hand_landmarks:
            for hand_landmarks in results.multi_hand_landmarks:
                # Landmark 4 = Thumb Tip
                # Landmark 12 = Middle Finger Tip
                thumb_tip = hand_landmarks.landmark[4]
                middle_tip = hand_landmarks.landmark[12]

                # Calculate Euclidean distance (in normalized 0-1 coordinates)
                # We prioritize 2D distance for speed and reliability in this view
                distance = math.sqrt(
                    (thumb_tip.x - middle_tip.x)**2 + 
                    (thumb_tip.y - middle_tip.y)**2
                )

                # Determine Pinch State
                is_pinched = distance < PINCH_THRESHOLD
                
                data_to_send["is_pinched"] = is_pinched
                data_to_send["distance"] = round(distance, 4)
                data_to_send["hand_detected"] = True

                # Visualization
                mp_draw.draw_landmarks(img, hand_landmarks, mp_hands.HAND_CONNECTIONS)
                
                # Draw line between fingers
                h, w, c = img.shape
                x1, y1 = int(thumb_tip.x * w), int(thumb_tip.y * h)
                x2, y2 = int(middle_tip.x * w), int(middle_tip.y * h)
                
                color = (0, 255, 0) if is_pinched else (0, 0, 255)
                cv2.line(img, (x1, y1), (x2, y2), color, 3)
                cv2.circle(img, (x1, y1), 10, color, cv2.FILLED)
                cv2.circle(img, (x2, y2), 10, color, cv2.FILLED)

        # Send Data via UDP
        # Format: JSON string for easy parsing in C#
        message = json.dumps(data_to_send)
        sock.sendto(message.encode(), (UDP_IP, UDP_PORT))

        # FPS Calculation
        curr_time = time.time()
        fps = 1 / (curr_time - prev_time) if prev_time != 0 else 0
        prev_time = curr_time
        
        cv2.putText(img, f"FPS: {int(fps)}", (10, 30), cv2.FONT_HERSHEY_SIMPLEX, 1, (255, 0, 0), 2)
        cv2.putText(img, f"Pinch: {data_to_send['is_pinched']}", (10, 70), cv2.FONT_HERSHEY_SIMPLEX, 1, (0, 255, 0) if data_to_send['is_pinched'] else (0, 0, 255), 2)

        cv2.imshow("Hand Tracking Sender", img)
        
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break

except Exception as e:
    print(f"Error: {e}")
finally:
    cap.release()
    cv2.destroyAllWindows()
    sock.close()
