using System;
using System.Collections;
using UnityEngine;

public class RunApp : MonoBehaviour
{
    #region MonoBehaviour
    private void Start()
    {
        GameMapManager.Instance.RelinkGameMap("Tiles_1");
        //StartCoroutine(delayCall(0.5f, ()=> GameMapManager.Instance.RelinkGameMap("Tiles_1")));
    }
    #endregion

    private IEnumerator delayCall(float sec, Action call)
    {
        yield return new WaitForSeconds(sec);
        call?.Invoke();
    }
}
