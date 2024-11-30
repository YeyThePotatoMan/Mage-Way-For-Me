using UnityEngine;
using UnityEngine.SceneManagement;


public class PausedMenu : MonoBehaviour

{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static bool paused = false; // Melacak apakah permainan sedang pause
    public GameObject PausedMenuCanvas; // Objek Canvas untuk menampilkan menu pause
    void Start()
    {
        Time.timeScale = 1f; 
            if (PausedMenuCanvas != null)
    {
        PausedMenuCanvas.SetActive(false); // Menonaktifkan menu pause saat game dimulai
    }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // Mengecek apakah tombol Escape ditekan
        {
            if (paused)
            {
                Play(); // Melanjutkan permainan jika sedang pause
            }
            else
            {
                Stop(); // Mem-pause permainan jika tidak sedang pause
            }
        }
    }
    public void Stop()
    {
        PausedMenuCanvas.SetActive(true); // Menampilkan Canvas menu pause
        Time.timeScale = 0f; // Menghentikan waktu dalam game
        paused = true; // Menandai bahwa permainan sedang pause
    }

    public void Play()
    {
        PausedMenuCanvas.SetActive(false); // Menyembunyikan Canvas menu pause
        Time.timeScale = 1f; // Melanjutkan waktu dalam game
        paused = false; // Menandai bahwa permainan tidak lagi pause
    }
    public void MainMenuButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1); // Memuat scene sebelumnya (main menu)
    }
    
}
