using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    [SerializeField] private AudioClip selectSound;
    public void LoadFieldScene()
    {
        SceneManager.LoadScene("Field");  // Make sure "Field" matches the exact name of your field scene
        SoundFXManager.Instance.PlaySound(selectSound);
    }
}