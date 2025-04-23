using System;
using System.Collections.Generic;
using Unity.VisualScripting;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Assets.Scripts.Utils.Interaction
{
        public static class InteractionUtils
        {
            private static readonly System.Random Rng = new System.Random();

            private static readonly bool KDebugCollidersFromRectTransform = false;

            public static bool CheckInteractiveObjectInList(InteractiveObject _io, List<InteractiveObject> _list)
            {
                foreach (InteractiveObject io in _list)
                {
                    if (io == _io)
                    {
                        return true;
                    }
                }
                return false;
            }

            public static bool IsEventHandlerRegistered(Delegate _prospectiveHandler, Action[] _handlers)
            {
                foreach (Delegate existing_handler in _handlers)
                {
                    if (existing_handler == _prospectiveHandler)
                    {
                        return true;
                    }
                }
                return false;
            }

            public static void Shuffle<T>(this IList<T> _list)
            {
                int n = _list.Count;
                while (n > 1)
                {
                    n--;
                    int k = Rng.Next(n + 1);
                    (_list[k], _list[n]) = (_list[n], _list[k]);
                }
            }


            public static T[] SubArray<T>(this T[] _data, int _index, int _length)
            {
                T[] result = new T[_length];
                Array.Copy(_data, _index, result, 0, _length);
                return result;
            }


            public static bool AreUnderSameGameObject(object _object1, object _object2)
            {
                return ConvertIntoGameObject(_object1).Equals(ConvertIntoGameObject(_object2));
            }

            /// <summary>
            /// Casts a generic object into an IO if this is a GameObject or a Monobehaivour.
            /// </summary>
            /// <param name="_obj">A Game Object or Monobehaivouir</param>
            /// <returns></returns>
            public static GameObject ConvertIntoGameObject(object _obj)
            {
                if (_obj is GameObject)
                    return (GameObject)_obj;

                if (_obj is MonoBehaviour)
                    return ((MonoBehaviour)_obj).gameObject;

                return null;
            }

            /// <summary>
            /// Checks if 2 objects are part of the same InteractiveObject object
            /// </summary>
            /// <returns></returns>
            public static bool AreUnderSameIO(object _obj1, object _obj2)
            {
                return GetIOFromObject(_obj1).Equals(GetIOFromObject(_obj2));
            }

            public static bool AreUnderSameIO(GameObject _obj1, object _obj2)
            {
                return GetIOFromGameObject(_obj1).Equals(GetIOFromObject(_obj2));
            }

            public static bool AreUnderSameIO(object _obj1, GameObject _obj2)
            {
                return GetIOFromObject(_obj1).Equals(GetIOFromGameObject(_obj2));
            }

            public static bool AreUnderSameIO(GameObject _obj1, GameObject _obj2)
            {
                return GetIOFromGameObject(_obj1).Equals(GetIOFromGameObject(_obj2));
            }

            public static InteractiveObject GetIOFromObject(object _obj)
            {
                return GetIOFromGameObject((GameObject)_obj);
            }

            public static InteractiveObject GetIOFromGameObject(GameObject _go, bool _includeInactive = false)
            {
                return _go.GetComponentInParent<InteractiveObject>(_includeInactive);
            }

            public static void EnsureFieldIsPositiveOrZero(ref float _val)
            {
                if (_val < 0)
                    _val = 0;
            }

            public static void EnsureFieldIsAboveLimit(ref float _val, float _limit)
            {
                if (_val < _limit)
                    _val = _limit;
            }

            /// <summary>
            /// Create a collider of the Game object's visuals mesh size
            /// </summary>
            /// <param name="_obj"></param>
            public static BoxCollider BoundColliderToMeshes(GameObject _obj)
            {
                // Unparent the object and set it's rotation to 0
                Transform last_parent = _obj.transform.parent;
                _obj.transform.SetParent(null);
                Quaternion last_rotation = _obj.transform.rotation;
                _obj.transform.rotation = Quaternion.identity;

                // Get the bounds of the object' meshes
                Bounds global_bounds = GetMinimumBoxOfSkinMesh(_obj);

                BoxCollider collider = _obj.GetOrAddComponent<BoxCollider>();
                collider.center = global_bounds.center;
                collider.size = global_bounds.size;

                // Rotate the object back
                _obj.transform.rotation = last_rotation;
                _obj.transform.SetParent(last_parent);

                return collider;
            }

            private const float KMinColliderThicknessFromRectTransform = 0.01f;
            public static Vector3 GetColliderSizeFromRectTransform(RectTransform _rt)
            {
                Vector3 size = Vector3.one;
                if (_rt != null)
                {
                    try
                    {
                        Vector3[] v = new Vector3[4];
                        _rt.GetWorldCorners(v);

                        size = new Vector3(Vector3.Magnitude(v[0] - v[3]), Vector3.Magnitude(v[0] - v[1]), KMinColliderThicknessFromRectTransform);

                        if (KDebugCollidersFromRectTransform)
                        {
                            for (int i = 0; i < v.Length; i++)
                            {
                                Debug.Log($"corner[{i}]: {v[i]:F3}");
                            }

                            Debug.DrawLine(v[0], v[1], Color.red, duration: 5f);
                            Debug.DrawLine(v[1], v[2], Color.green, duration: 5f);
                            Debug.DrawLine(v[2], v[3], Color.blue, duration: 5f);
                            Debug.DrawLine(v[3], v[0], Color.cyan, duration: 5f);

                            Debug.Log($"GetColliderDimensionsFromRectTransform size: {size:F3}");
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
                else
                {
                    Debug.LogError($"GetColliderDimensionsFromRectTransform encountered null RectTransform!");
                }

                return size;
            }

            public static void SetColliderSizeFromRectTransform(RectTransform _rt, ref BoxCollider _bc)
            {
                if (_rt != null)
                {
                    if (_bc != null)
                    {
                        Vector3 size = GetColliderSizeFromRectTransform(_rt);
                        _bc.size = new Vector3(size.x / _bc.transform.lossyScale.x,
                            size.y / _bc.transform.lossyScale.y, size.z / _bc.transform.lossyScale.z);
                    }
                    else
                    {
                        Debug.LogError($"SetColliderSizeFromRectTransform encountered null BoxCollider!");
                    }
                }
                else
                {
                    Debug.LogError($"SetColliderSizeFromRectTransform encountered null RectTransform!");
                }
            }

            /// <summary>
            /// Gets the minimum bounds of a group of meshes
            /// </summary>
            /// <param name="_obj"></param>
            /// <returns></returns>
            public static Bounds GetMinimumBoxOfSkinMesh(GameObject _obj)
            {
                Renderer[] meshes = _obj.GetComponentsInChildren<Renderer>();
                Bounds bounds;

                float max_x = float.MinValue;
                float min_x = float.MaxValue;

                float max_y = float.MinValue;
                float min_y = float.MaxValue;

                float max_z = float.MinValue;
                float min_z = float.MaxValue;

                for (int i = 0; i < meshes.Length; i++)
                {
                    bounds = meshes[i].bounds;

                    if (bounds.min.x < min_x)
                    {
                        min_x = bounds.min.x;
                    }

                    if (bounds.max.x > max_x)
                    {
                        max_x = bounds.max.x;
                    }

                    if (bounds.min.y < min_y)
                    {
                        min_y = bounds.min.y;
                    }

                    if (bounds.max.y > max_y)
                    {
                        max_y = bounds.max.y;
                    }

                    if (bounds.min.z < min_z)
                    {
                        min_z = bounds.min.z;
                    }

                    if (bounds.max.z > max_z)
                    {
                        max_z = bounds.max.z;
                    }
                }

                Vector3 min = new Vector3(min_x, min_y, min_z);
                Vector3 max = new Vector3(max_x, max_y, max_z);

                Vector3 min_to_max = max - min;
                Vector3 center = min + min_to_max / 2f;

                Bounds to_return = new Bounds()
                {
                    center = center,
                    size = min_to_max
                };
                return to_return;
            }

            /// <summary>
            /// Makes a mesh match a container size (the container needs to have a mesh or a group of meshes)
            /// </summary>
            /// <param name="_target"> The object to scale </param>
            /// <param name="_container"> The container (needs to have a mesh) </param>
            /// <param name="_moveToTransform"> Do we need to move the object inside the container </param>
            public static void ScaleMeshToContainer(GameObject _target, GameObject _container, bool _moveToTransform)
            {
                List<Transform> chilldren = new List<Transform>();
                Transform child;
                if (_container.transform.childCount != 0)
                {

                    for (int i = _container.transform.childCount - 1; i >= 0; --i)
                    {
                        child = _container.transform.GetChild(i);
                        chilldren.Add(child);
                        child.SetParent(null);

                    }
                }

                // First Scale the object to match the container dimensions
                Bounds target_bounds = GetMinimumBoxOfSkinMesh(_target);
                Bounds container_bounds = GetMinimumBoxOfSkinMesh(_container);

                Vector3 compress_ratio = new Vector3(
                        container_bounds.size.x / target_bounds.size.x,
                        container_bounds.size.y / target_bounds.size.y,
                        container_bounds.size.z / target_bounds.size.z);

                _target.transform.localScale *= GetMinCoordinateOf(compress_ratio);


                if (!_moveToTransform)
                    return;

                // Move the object to the correct position using the bounds offsets
                // Important to compensate the pivot points
                _target.transform.position = _container.transform.position;
                target_bounds = GetMinimumBoxOfSkinMesh(_target);
                container_bounds = GetMinimumBoxOfSkinMesh(_container);
                _target.transform.position += container_bounds.center - target_bounds.center;


                for (int i = chilldren.Count - 1; i >= 0; --i)
                {
                    child = chilldren[i];
                    child.SetParent(_container.transform);

                }
            }

            public static float GetMaxCoordinateOf(Vector3 _vector)
            {
                return Mathf.Max(Mathf.Max(_vector.x, _vector.y), _vector.z);
            }

            public static float GetMinCoordinateOf(Vector3 _vector)
            {
                return Mathf.Min(Mathf.Min(_vector.x, _vector.y), _vector.z);
            }


            /// <summary>
            /// Gets a list of objects that have been raycasted and contains the object especified by T
            /// </summary>
            /// <typeparam name="T"> Desired types </typeparam>
            /// <param name="_position"> Origin of the raycast </param>
            /// <param name="_direction"> Direction of the ray </param>
            /// <param name="_maxDistance"> Max detection distance </param>
            /// <param name="_sortByDistance"> Do we want the objects sorted by detection distance </param>
            /// <param name="_layerMask"> Mask to target when performing the raycast </param>
            /// <returns></returns>
            public static List<T> RaycastOnlyObjectsContaining<T>(Vector3 _position, Vector3 _direction, float _maxDistance, bool _sortByDistance = false, int _layerMask = -1) where T : Component
            {
                RaycastHit[] hits = Raycast(_position, _direction, _maxDistance, _layerMask, _sortByDistance);

                return hits.Length == 0 ? new List<T>() : GetHitsOnlyContaining<T>(hits);
            }

            /// <summary>
            /// Gets the objects from the list containing a certrain component
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="_hits"></param>
            /// <returns></returns>
            public static List<T> GetHitsOnlyContaining<T>(RaycastHit[] _hits) where T : Component
            {
                List<T> to_return = new List<T>();

                for (int i = 0; i < _hits.Length; i++)
                {
                    var obj = _hits[i].collider.GetComponent<T>();
                    if (obj != null)
                        to_return.Add(obj);
                }

                return to_return;
            }

            public static RaycastHit[] Raycast(Vector3 _position, Vector3 _direction, float _maxDistance, int _layer, bool _sort)
            {
                RaycastHit[] hits;

                hits = _layer == -1
                    ? Physics.RaycastAll(_position, _direction, _maxDistance)
                    : Physics.RaycastAll(_position, _direction, _maxDistance, _layer);

                if (_sort)
                    hits.RaycastSort(_position, _direction);

                return hits;
            }

            public static RaycastHit[] SphereCast(Vector3 _position, Vector3 _direciton, float _maxDistance, int _layer, bool _sort)
            {
                RaycastHit[] hits;

                hits = _layer != -1
                    ? Physics.SphereCastAll(_position, _maxDistance, _direciton, _layer)
                    : Physics.SphereCastAll(_position, _maxDistance, _direciton);

                if (_sort)
                    hits.RaycastSort(_position, _direciton);

                return hits;
            }

            public static void RaycastSort(this RaycastHit[] _hits, Vector3 _origin, Vector3 _direction)
            {
                // Bubble sort the hits
                var ray = new Ray(_origin, _direction);
                for (var i = 0; i < _hits.Length; i++)
                {
                    for (var j = 0; j < _hits.Length - 1; j++)
                    {
                        // NOTE(4nc3str4l): Get the distance from the raycast line to the center for each element
                        // As closer of the center the more interest of the user to select it. (Distance doesn't effect)
                        var d1 = Vector3.Cross(ray.direction, _hits[j].collider.bounds.center - ray.origin).magnitude;
                        var d2 = Vector3.Cross(ray.direction, _hits[j + 1].collider.bounds.center - ray.origin).magnitude;
                        if (d1 > d2)
                        {
                            (_hits[j + 1], _hits[j]) = (_hits[j], _hits[j + 1]);
                        }
                    }
                }
            }

            /// <summary>
            /// Gets the elements from the A list that are not on the B list
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="_A"></param>
            /// <param name="_B"></param>
            /// <returns></returns>
            public static List<T> GetElementsFromANotInB<T>(List<T> _A, List<T> _B)
            {
                List<T> elements_a_not_in_b = new List<T>();

                int counter = _A.Count;
                for (int i = 0; i < counter; i++)
                {
                    T element = _A[i];
                    if (!_B.Contains(element))
                    {
                        elements_a_not_in_b.Add(element);
                    }
                }

                return elements_a_not_in_b;
            }

            public static float GetClosestToNumberInList(float _num, float[] _list)
            {
                float to_return = -1;
                float min_distance = float.MaxValue;
                float tmp_distance = float.MaxValue;
                foreach (var t in _list)
                {
                    tmp_distance = Mathf.Abs(t - _num);
                    if (tmp_distance < min_distance)
                    {
                        to_return = t;
                        min_distance = tmp_distance;
                    }
                }
                return to_return;
            }
            /// <summary>
            /// Convert a number belonging to interval A to its correspective in interval B
            /// </summary>
            /// <param name="_aNum"></param>
            /// <param name="_aMin"></param>
            /// <param name="_aMax"></param>
            /// <param name="_bMin"></param>
            /// <param name="_bMax"></param>
            /// <returns></returns>

            public static float ConvertNumberFromIntevalAToB(float _aNum, float _aMin, float _aMax, float _bMin, float _bMax)
            {
                return _bMin + ((_aNum - _aMin) / (_aMax - _aMin)) * (_bMax - _bMin);
            }

            public static List<T> Shuffle<T>(List<T> _list)
            {
                System.Random rnd = new System.Random();
                for (int i = 0; i < _list.Count; i++)
                {
                    int k = rnd.Next(0, i);
                    (_list[k], _list[i]) = (_list[i], _list[k]);
                }
                return _list;
            }
        }

        public static class ExtensionMethods
        {

            public static float Remap(this float _value, float _from1, float _to1, float _from2, float _to2)
            {
                return (_value - _from1) / (_to1 - _from1) * (_to2 - _from2) + _from2;
            }
        }

        public class ReadOnlyAttribute : PropertyAttribute { }



#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(_position, _property, _label, true);
            GUI.enabled = true;
        }
    }
#endif
}
