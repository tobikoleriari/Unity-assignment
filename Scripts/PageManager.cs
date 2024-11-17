using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageManager : MonoBehaviour
{
    public int frameSize = 3; // Number of memory frames
    public List<int> pageQueue = new List<int> { 7, 0, 1, 2, 0, 3, 0, 4 }; // Sample page requests
    private Queue<int> frames; // Queue to hold current pages in memory
    public Transform memoryPanel; // Reference to MemoryPanel (where frame slots are)
    public Transform queuePanel; // Reference to PageQueuePanel (where the page queue is displayed)
    public Text statsText; // Text to display hits and faults
    private int hits = 0;
    private int faults = 0;

    private Text[] frameTexts; // Array to store references to text components in memory frames

    public float delayTime = 0.5f; // Delay time between page requests (in seconds)

    void Start()
    {
        // Initialize the frame queue and update the UI
        frames = new Queue<int>(frameSize);
        DisplayStats();

        // Get all the Text components inside the memoryPanel slots
        frameTexts = memoryPanel.GetComponentsInChildren<Text>();

        // Ensure that the frameTexts array has the correct size
        if (frameTexts.Length != frameSize)
        {
            Debug.LogError("The number of frame slots does not match the specified frame size.");
        }

        InitializeQueue();
        StartSimulation(); // Start the simulation
    }

    // Starts the simulation to process page requests
    private void StartSimulation()
    {
        StartCoroutine(ProcessQueue()); // Process each page request with a delay
    }

    // Processes all pages in the queue with a delay between each
    private IEnumerator ProcessQueue()
    {
        foreach (int page in pageQueue)
        {
            ProcessPage(page); // Process each page request
            yield return new WaitForSeconds(delayTime); // Wait for the specified delay time
        }
    }

    // Processes a single page request (hit or fault)
    public void ProcessPage(int page)
    {
        if (frames.Contains(page)) // If the page is already in memory (hit)
        {
            hits++;
            HighlightFrame(page, Color.green); // Highlight frame in green for hit
        }
        else // If the page is not in memory (fault)
        {
            faults++;
            if (frames.Count == frameSize) // If memory is full, remove the oldest page
            {
                int removedPage = frames.Dequeue();
                RemoveFrame(removedPage);
            }
            frames.Enqueue(page); // Add the new page to memory
            AddFrame(page, Color.red); // Add the page with red color for a fault
        }
        DisplayStats(); // Update the stats display
    }

    // Updates the stats text showing hits and faults
    private void DisplayStats()
    {
        statsText.text = $"Hits: {hits} | Faults: {faults}";
    }

    // Initializes the visual page queue in the UI
    private void InitializeQueue()
    {
        // Clear existing queue visuals
        foreach (Transform child in queuePanel)
        {
            Destroy(child.gameObject);
        }

        // Add the page requests to the queue panel visually
        foreach (int page in pageQueue)
        {
            GameObject pageObj = new GameObject($"Page_{page}");
            pageObj.transform.SetParent(queuePanel, false); // Set parent to PageQueuePanel
            var text = pageObj.AddComponent<Text>();
            text.text = page.ToString();
            text.fontSize = 24;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = Color.black; // Set text color to black

            // Optional: Add a small margin between page request items
            var layoutElement = pageObj.AddComponent<LayoutElement>();
            layoutElement.minWidth = 50;
        }
    }

    // Adds a page to the memory frames visually (only Text)
    private void AddFrame(int page, Color color)
    {
        for (int i = 0; i < frameSize; i++)
        {
            // If the current frame is empty (no page inside it)
            if (string.IsNullOrEmpty(frameTexts[i].text))
            {
                frameTexts[i].text = page.ToString(); // Set the page text in the empty slot
                frameTexts[i].color = color; // Set color to red for fault
                break; // Stop once the page is added to the first empty slot
            }
        }
    }

    // Removes a page from memory visually
    private void RemoveFrame(int page)
    {
        for (int i = 0; i < frameSize; i++)
        {
            // If the current frame contains the specified page
            if (frameTexts[i].text == page.ToString())
            {
                frameTexts[i].text = ""; // Clear the text to indicate the frame is empty
                break; // Stop once the page is removed
            }
        }
    }

    // Highlights a memory frame to indicate a page hit
    private void HighlightFrame(int page, Color color)
    {
        for (int i = 0; i < frameSize; i++)
        {
            if (frameTexts[i].text == page.ToString())
            {
                frameTexts[i].color = color; // Change color to indicate hit (green)
                break; // Stop once the page is highlighted
            }
        }
    }
}
