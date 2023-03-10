using System;
using System.Collections;
using UnityEngine;

public class Role : MonoBehaviour, IMovable
{
    [SerializeField] [Range(1, 7)] private int _moveRange = 3;
    [SerializeField] [Range(1, 3)] private float _moveSpeed = 2;

    private Action _onMoveEnd;

    #region implement IMovable
    public Vector2 MapPosition => transform.position;
    public int MoveRange => _moveRange;

    public void Move(Transform trTarget, Action onMoveEnd)
    {
        _onMoveEnd = onMoveEnd;
        StartCoroutine(moveToSimple(trTarget));
    }
    #endregion

    #region MonoBehaviour
    private void OnMouseDown()
    {
        ToggleMovableTiles();
    }
    #endregion

    private void ToggleMovableTiles()
    {
        GameMapManager.Instance.ToggleMovableTiles(this, MapPosition, _moveRange);
    }

    /// <summary>
    /// Move distance of per frame = speed * Time.deltaTime
    /// Using Vector2.MoveTowards(...): https://docs.unity3d.com/ScriptReference/Vector2.MoveTowards.html
    /// Row first and column move later.
    /// </summary>
    private IEnumerator moveToSimple(Transform trTarget)
    {
        while(transform.position.x != trTarget.position.x)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(trTarget.position.x, transform.position.y), _moveSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        while (transform.position.y != trTarget.position.y)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, trTarget.position.y), _moveSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        _onMoveEnd?.Invoke();
    }
}
