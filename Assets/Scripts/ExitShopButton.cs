using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitShopButton : MonoBehaviour
{
    [SerializeField] private AudioClip selectSound;
    public void LoadFieldScene()
    {
        SoundFXManager.Instance.PlaySound(selectSound);
        SceneManager.LoadScene("Field");  // Make sure "Field" matches the exact name of your field scene
    }
}