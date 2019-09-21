using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithInput : MonoBehaviour 
{
    public GameObject Follow;
	public float Speed = 1.0f;

    public Rect Rect;

    void Awake()
    {        
        StartCoroutine(UpdateTarget());
    }

	IEnumerator UpdateTarget() 
	{
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                Follow = null;
                transform.position = Vector3.zero;
            }

            var move = Vector3.zero;

            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                move.y += 1;
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                move.y -= 1;
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                move.x -= 1;
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                move.x += 1;

            var offset = move * Speed * Time.unscaledDeltaTime;

            if (offset == Vector3.zero)
            {
                if (Follow != null)
                    transform.position = Follow.transform.position;
            }
            else if (Follow != null)
                Follow = null;

            transform.position += offset;

            if (!Rect.Contains(transform.position))
            {
                var bounds = new Bounds(Rect.center, Rect.size);
                transform.position = bounds.ClosestPoint(transform.position);
            }

            // if(Follow == null)
            // {
            //     FollowArrow.SetActive(false);
            // }
            // else
            // {
            //     FollowArrow.SetActive(true);
            //     FollowArrow.transform.position = Follow.transform.position + Vector3.up * 2;
            // }

            yield return null;
        }
	}

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(Rect.center, Rect.size);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}

