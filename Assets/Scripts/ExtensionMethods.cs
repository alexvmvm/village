using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods 
{
    public static string ToUppercaseFirst(this string s)
    {
        // Check for empty string.
        if (string.IsNullOrEmpty(s))
        {
            return string.Empty;
        }
        // Return char and concat substring.
        return char.ToUpper(s[0]) + s.Substring(1);
    }

    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        HashSet<TKey> seenKeys = new HashSet<TKey>();
        foreach (TSource element in source)
        {
            if (seenKeys.Add(keySelector(element)))
            {
                yield return element;
            }
        }
    }

    public static bool IsSubsetOf<TKey, TValue>(this IDictionary<TKey, TValue> current, IDictionary<TKey, TValue> target)
	{
		foreach(var kv in current)
		{
			if(!target.ContainsKey(kv.Key) || !target[kv.Key].Equals(kv.Value))
				return false;
		}
		return true;
	}

    public static bool IsEqualTo<TKey, TValue>(this IDictionary<TKey, TValue> current, IDictionary<TKey, TValue> target)
    {
        if (current.Count != target.Count)
            return false;

        foreach (var kv in current)
        {
            if (!target.ContainsKey(kv.Key) || !target[kv.Key].Equals(kv.Value))
                return false;
        }
        return true;
    }

    public static Vector2Int ToVector2Int(this Vector2 vector)
	{
		return new Vector2Int (Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y));
	}

    public static bool AdjacentTo(this Vector2Int vector, Vector2Int target)
    {
        return vector.Left() == target || vector.Right() == target || vector.Up() == target || vector.Down() == target;
    }

    public static Vector2Int Left(this Vector2Int vector)
    {
        return new Vector2Int(vector.x - 1, vector.y);
    }

    public static Vector2Int Right(this Vector2Int vector)
    {
        return new Vector2Int(vector.x + 1, vector.y);
    }

    public static Vector2Int Down(this Vector2Int vector)
    {
        return new Vector2Int(vector.x, vector.y - 1);
    }

    public static Vector2Int Up(this Vector2Int vector)
    {
        return new Vector2Int(vector.x, vector.y + 1);
    }

    public static Vector3Int ToVector3IntFloor(this Vector2 vector)
    {
        return new Vector3Int(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y), 0);
    }

    public static Vector2Int ToVector2IntFloor(this Vector2 vector)
    {
        return new Vector2Int(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y));
    }

    public static Vector3 ToVector3(this Vector2 vector)
    {
        return new Vector3(vector.x, vector.y);
    }

    public static Vector2 ToVector2(this Vector3 vector)
    {
        return new Vector2(vector.x, vector.y);
    }

    public static Vector3 ToVector3Half(this Vector3 vector)
    {
        return new Vector3(Mathf.RoundToInt(vector.x) + 0.5f, Mathf.RoundToInt(vector.y) + 0.5f);
    }

    public static Vector3 ToVector(this Vector3Int vector)
	{
		return new Vector3 (vector.x, vector.y, vector.z);
	}

    public static Vector3 RotatePointAroundPivot(this Vector3 point, Vector3 pivot, Quaternion angle)
    {
        return angle * (point - pivot) + pivot;
    }

    public static Vector3Int ToVectorInt(this Vector3 vector)
	{
		return new Vector3Int (
			Mathf.RoundToInt(vector.x), 
			Mathf.RoundToInt(vector.y), 
			Mathf.RoundToInt(vector.z));
	}

	public static Vector3Int ToVectorIntFloor(this Vector3 vector)
	{
		return new Vector3Int (
			Mathf.FloorToInt(vector.x), 
			Mathf.FloorToInt(vector.y), 
			Mathf.FloorToInt(vector.z));
	}


    public static Vector2Int ToVector2IntFloor(this Vector3 vector)
    {
        return new Vector2Int(
            Mathf.FloorToInt(vector.x),
            Mathf.FloorToInt(vector.y));
    }

    public static Vector2Int ToVector2Int(this Vector3Int vector)
	{
		return new Vector2Int(vector.x, vector.y);
	}

    public static Vector2 ToVector(this Vector2Int vector)
    {
        return new Vector2(vector.x, vector.y);
    }

    public static Vector3Int ToVector3Int(this Vector2Int vector)
	{
		return new Vector3Int (
			vector.x, 
			vector.y, 
			0);
	}

	public static Vector3 ToVector3(this Vector2Int vector)
	{
		return new Vector3 (
			vector.x, 
			vector.y, 
			0);
	}

	public static Vector3Int SwapXY(this Vector3Int vector)
	{
		return new Vector3Int (vector.y, vector.x, vector.z);
	}

    public static Vector3 XYPosition(this Transform transform)
    {
        return new Vector3(transform.position.x, transform.position.y);
    }

    public static Color MoveTowards(this Color current, Color target, float maxDistanceDelta)
    {
        return new Color(
            Mathf.MoveTowards(current.r, target.r, maxDistanceDelta),
            Mathf.MoveTowards(current.g, target.g, maxDistanceDelta),
            Mathf.MoveTowards(current.b, target.b, maxDistanceDelta),
            Mathf.MoveTowards(current.a, target.a, maxDistanceDelta)
            );
    }


    public static string UppercaseFirst(this string s)
	{
		// Check for empty string.
		if (string.IsNullOrEmpty(s))
		{
			return string.Empty;
		}

		// Return char and concat substring.
		return char.ToUpper(s[0]) + s.Substring(1);
	}

    private static System.Random random = new System.Random();

    public static IList<T> Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = random.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }

        return list;
    }

    public static void Delete<T>(this LinkedList<T> list, LinkedListNode<T> node)
    {
        for (var current = list.First; current != null; current = current.Next)
        {
            if (current == node)
            {
                if (current.Previous == null && current.Next == null)
                    list.Clear();
                else if (current.Previous != null && current.Next != null)
                    list.AddAfter(current.Previous, current.Next);
                else if (current.Previous == null)
                    list.RemoveFirst();
                else if (current.Next == null)
                    list.RemoveLast();

                return;
            }
        }
    }

    public static Rect GetWorldRect (this RectTransform rt, Vector2 scale) {
         // Convert the rectangle to world corners and grab the top left
         Vector3[] corners = new Vector3[4];
         rt.GetWorldCorners(corners);
         Vector3 topLeft = corners[0];
 
         // Rescale the size appropriately based on the current Canvas scale
         Vector2 scaledSize = new Vector2(scale.x * rt.rect.size.x, scale.y * rt.rect.size.y);
 
         return new Rect(topLeft, scaledSize);
     }

    public static T GetAttributeOfType<T>(this Enum enumVal) where T : System.Attribute
    {
        var type = enumVal.GetType();
        var memInfo = type.GetMember(enumVal.ToString());
        var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
        return (attributes.Length > 0) ? (T)attributes[0] : null;
    }


    public static T GetComponentInChildrenExcludingParent<T>(this Component target) where T : MonoBehaviour
    {
        foreach(var c in target.GetComponentsInChildren<T>())
        {
            if (c.gameObject == target.gameObject) continue;
            return c;
        }

        return null;
    }

    public static T GetComponentInParentExcludingCurrent<T>(this Component target) where T : MonoBehaviour
    {
        foreach(var c in target.GetComponentsInParent<T>())
        {
            if (c.gameObject == target.gameObject) continue;
            return c;
        }

        return null;
    }

    public static Rect ToRect(this Bounds bounds)
    {
        return new Rect(bounds.min, bounds.size);
    }

    public static Rect ToRect(this RectInt rect)
    {
        return new Rect(rect.position, rect.size);
    }

    public static RectInt ToRectInt(this Rect rect)
    {
        return new RectInt(rect.position.ToVector2Int(), rect.size.ToVector2Int());
    }

    public static bool AdjacentTo(this RectInt a, RectInt b)
    {
        return 
            (a.max.x == b.min.x && a.max.y > b.min.y && a.min.y < b.max.y) ||
            (a.min.x == b.max.x && a.max.y > b.min.y && a.min.y < b.max.y) ||
            (a.max.y == b.min.y && a.max.x > b.min.x && a.min.x < b.max.x) ||
            (a.min.y == b.max.y && a.max.x > b.min.x && a.min.x < b.max.x);
    }

    public static bool AdjacentTo(this RectInt a, Vector2Int b)
    {
        return 
            a.Contains(b + Vector2Int.up) || 
            a.Contains(b + Vector2Int.down) || 
            a.Contains(b + Vector2Int.left) || 
            a.Contains(b + Vector2Int.right);
    }

    public static bool Contains(this RectInt a, int x, int y)
    {
        return x >= a.min.x && x < a.max.x && y >= a.min.y && y < a.max.y;
    }

    public static Vector2Int[] ToPointsArray(this RectInt rect)
    {
        var array = new Vector2Int[rect.width * rect.height];
        var idx = 0;
        for(var x = rect.min.x; x < rect.max.x; x++) 
        {
            for(var y = rect.min.y; y < rect.max.y; y++)
            {
                array[idx] = new Vector2Int(x, y);
                idx++;
            }
        }

        return array;
    }
        
    public static Vector2Int RandomVector2Int(this RectInt rect)
    {
        return new Vector2Int(
            UnityEngine.Random.Range(rect.min.x, rect.max.x),
            UnityEngine.Random.Range(rect.min.y, rect.max.y));
    }

    
    public static Bounds ToBounds(this Rect rect)
    {
        return new Bounds(rect.position, rect.size);
    }

    public static Bounds ToBounds(this BoundsInt bounds)
    {
        return new Bounds(bounds.center, bounds.size);
    }

    private static Color[] _colors = new Color[]
    {
        Color.black,
        Color.blue,
        Color.cyan,
        Color.green,
        Color.grey,
        Color.magenta,
        Color.red,
        Color.white,
        Color.yellow
    };

    public static Color RandomColorFromInt(int index)
    {
        return _colors[index % _colors.Length];
    }

    public static Color RandomColor()
    {
        return _colors[UnityEngine.Random.Range(0, _colors.Length)];
    }
 
}
