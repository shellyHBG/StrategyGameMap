using UnityEngine;

public class RunApp : MonoBehaviour
{
    #region MonoBehaviour
    private void Start()
    {
        Debug.Log($"Game Start！");
        GameMapManager.Instance.RelinkGameMap("Tiles_1");
    }
    #endregion
}
