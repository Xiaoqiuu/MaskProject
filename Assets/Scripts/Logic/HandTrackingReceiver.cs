using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine.Events;

[System.Serializable]
public class HandData
{
    public bool is_pinched;
    public float distance;
    public bool hand_detected;
}

public class HandTrackingReceiver : MonoBehaviour
{
    [Header("Network Settings")]
    public int port = 5052;
    
    [Header("Debug")]
    [SerializeField] private bool isPinched;
    [SerializeField] private float currentDistance;
    [SerializeField] private bool handDetected;

    [Header("Events")]
    public UnityEvent OnPinchStart;
    public UnityEvent OnPinchEnd;

    private Thread receiveThread;
    private UdpClient client;
    private bool isRunning = true;

    // Data synchronization
    private HandData latestData = null;
    private object dataLock = new object();
    
    // State tracking for events
    private bool wasPinched = false;

    void Start()
    {
        latestData = new HandData();
        StartReceiver();
    }

    void StartReceiver()
    {
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
        Debug.Log($"[HandTracking] UDP Receiver started on port {port}");
    }

    void ReceiveData()
    {
        client = new UdpClient(port);
        IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);

        while (isRunning)
        {
            try
            {
                byte[] data = client.Receive(ref anyIP);
                string text = Encoding.UTF8.GetString(data);

                // Parse JSON
                HandData receivedData = JsonUtility.FromJson<HandData>(text);

                lock (dataLock)
                {
                    latestData = receivedData;
                }
            }
            catch (System.Exception err)
            {
                if (isRunning) Debug.LogError($"[HandTracking] Error: {err.Message}");
            }
        }
    }

    void Update()
    {
        HandData currentData = null;

        // Thread-safe read
        lock (dataLock)
        {
            if (latestData != null)
            {
                currentData = latestData;
            }
        }

        if (currentData != null)
        {
            isPinched = currentData.is_pinched;
            currentDistance = currentData.distance;
            handDetected = currentData.hand_detected;

            // Trigger events on state change
            if (isPinched && !wasPinched)
            {
                OnPinchStart.Invoke();
                Debug.Log("Pinch Started");
            }
            else if (!isPinched && wasPinched)
            {
                OnPinchEnd.Invoke();
                Debug.Log("Pinch Ended");
            }

            wasPinched = isPinched;
        }
    }

    void OnDestroy()
    {
        isRunning = false;
        if (client != null) client.Close();
        if (receiveThread != null && receiveThread.IsAlive) receiveThread.Abort();
    }
}
