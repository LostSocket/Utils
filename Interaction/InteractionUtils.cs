﻿using System;
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
            private static System.Random rng = new System.Random();

            private static readonly bool kDebugCollidersFromRectTransform = false;

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

            public static bool IsEventHandlerRegistered(Delegate prospectiveHandler, Action[] _handlers)
            {
                foreach (Delegate existingHandler in _handlers)
                {
                    if (existingHandler == prospectiveHandler)
                    {
                        return true;
                    }
                }
                return false;
            }

            public static void Shuffle<T>(this IList<T> list)
            {
                int n = list.Count;
                while (n > 1)
                {
                    n--;
                    int k = rng.Next(n + 1);
                    T value = list[k];
                    list[k] = list[n];
                    list[n] = value;
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

            public static InteractiveObject GetIOFromGameObject(GameObject _go, bool includeInactive = false)
            {
                return _go.GetComponentInParent<InteractiveObject>(includeInactive);
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
                Transform lastParent = _obj.transform.parent;
                _obj.transform.SetParent(null);
                Quaternion lastRotation = _obj.transform.rotation;
                _obj.transform.rotation = Quaternion.identity;

                // Get the bounds of the object' meshes
                Bounds globalBounds = GetMinimumBoxOfSkinMesh(_obj);

                BoxCollider collider = _obj.GetOrAddComponent<BoxCollider>();
                collider.center = globalBounds.center;
                collider.size = globalBounds.size;

                // Rotate the object back
                _obj.transform.rotation = lastRotation;
                _obj.transform.SetParent(lastParent);

                return collider;
            }

            private const float kMinColliderThicknessFromRectTransform = 0.01f;
            public static Vector3 GetColliderSizeFromRectTransform(RectTransform rt)
            {
                Vector3 size = Vector3.one;
                if (rt != null)
                {
                    try
                    {
                        Vector3[] v = new Vector3[4];
                        rt.GetWorldCorners(v);

                        size = new Vector3(Vector3.Magnitude(v[0] - v[3]), Vector3.Magnitude(v[0] - v[1]), kMinColliderThicknessFromRectTransform);

                        if (kDebugCollidersFromRectTransform)
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

            public static void SetColliderSizeFromRectTransform(RectTransform rt, ref BoxCollider bc)
            {
                if (rt != null)
                {
                    if (bc != null)
                    {
                        Vector3 size = GetColliderSizeFromRectTransform(rt);
                        bc.size = new Vector3(size.x / bc.transform.lossyScale.x,
                            size.y / bc.transform.lossyScale.y, size.z / bc.transform.lossyScale.z);
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
                Bounds bounds = new Bounds();

                float maxX = float.MinValue;
                float minX = float.MaxValue;

                float maxY = float.MinValue;
                float minY = float.MaxValue;

                float maxZ = float.MinValue;
                float minZ = float.MaxValue;

                for (int i = 0; i < meshes.Length; i++)
                {
                    bounds = meshes[i].bounds;

                    if (bounds.min.x < minX)
                    {
                        minX = bounds.min.x;
                    }

                    if (bounds.max.x > maxX)
                    {
                        maxX = bounds.max.x;
                    }

                    if (bounds.min.y < minY)
                    {
                        minY = bounds.min.y;
                    }

                    if (bounds.max.y > maxY)
                    {
                        maxY = bounds.max.y;
                    }

                    if (bounds.min.z < minZ)
                    {
                        minZ = bounds.min.z;
                    }

                    if (bounds.max.z > maxZ)
                    {
                        maxZ = bounds.max.z;
                    }
                }

                Vector3 min = new Vector3(minX, minY, minZ);
                Vector3 max = new Vector3(maxX, maxY, maxZ);

                Vector3 minToMax = max - min;
                Vector3 center = min + minToMax / 2f;

                Bounds toReturn = new Bounds()
                {
                    center = center,
                    size = minToMax
                };
                return toReturn;
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
                Bounds targetBounds = GetMinimumBoxOfSkinMesh(_target);
                Bounds containerBounds = GetMinimumBoxOfSkinMesh(_container);

                Vector3 compressRatio = new Vector3(
                        containerBounds.size.x / targetBounds.size.x,
                        containerBounds.size.y / targetBounds.size.y,
                        containerBounds.size.z / targetBounds.size.z);

                _target.transform.localScale *= GetMinCoordinateOf(compressRatio);


                if (!_moveToTransform)
                    return;

                // Move the object to the correct position using the bounds offsets
                // Important to compensate the pivot points
                _target.transform.position = _container.transform.position;
                targetBounds = GetMinimumBoxOfSkinMesh(_target);
                containerBounds = GetMinimumBoxOfSkinMesh(_container);
                _target.transform.position += containerBounds.center - targetBounds.center;


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
            /// <returns></returns>
            public static List<T> RaycastOnlyObjectsContaining<T>(Vector3 _position, Vector3 _direction, float _maxDistance, bool _sortByDistance = false, int _layerMask = -1) where T : Component
            {
                RaycastHit[] hits = Raycast(_position, _direction, _maxDistance, _layerMask, _sortByDistance);

                if (hits.Length == 0)
                    return new List<T>();

                return GetHitsOnlyContaining<T>(hits);
            }

            /// <summary>
            /// Gets the objects from the list containing a certrain component
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="_hits"></param>
            /// <returns></returns>
            public static List<T> GetHitsOnlyContaining<T>(RaycastHit[] _hits) where T : Component
            {
                List<T> toReturn = new List<T>();

                T obj = null;
                for (int i = 0; i < _hits.Length; i++)
                {
                    obj = _hits[i].collider.GetComponent<T>();
                    if (obj != null)
                        toReturn.Add(obj);
                }

                return toReturn;
            }

            public static RaycastHit[] Raycast(Vector3 _position, Vector3 _direction, float _maxDistance, int _layer, bool _sort)
            {
                RaycastHit[] hits;

                if (_layer == -1)
                    hits = Physics.RaycastAll(_position, _direction, _maxDistance);
                else
                    hits = Physics.RaycastAll(_position, _direction, _maxDistance, _layer);

                if (_sort)
                    hits.RaycastSort(_position, _direction);

                return hits;
            }

            public static RaycastHit[] SphereCast(Vector3 _position, Vector3 _direciton, float _maxDistance, int _layer, bool _sort)
            {
                RaycastHit[] hits;

                if (_layer != -1)
                    hits = Physics.SphereCastAll(_position, _maxDistance, _direciton, _layer);
                else
                    hits = Physics.SphereCastAll(_position, _maxDistance, _direciton);

                if (_sort)
                    hits.RaycastSort(_position, _direciton);

                return hits;
            }

            public static void RaycastSort(this RaycastHit[] hits, Vector3 _origin, Vector3 _direction)
            {
                // Bubble sort the hits
                RaycastHit temp;
                float d1, d2;
                Ray ray = new Ray(_origin, _direction);
                for (int i = 0; i < hits.Length; i++)
                {
                    for (int j = 0; j < hits.Length - 1; j++)
                    {
                        // NOTE(4nc3str4l): Get the distance from the raycast line to the center for each element
                        // As closer of the center the more interest of the user to select it. (Distance doesn't effect)
                        d1 = Vector3.Cross(ray.direction, hits[j].collider.bounds.center - ray.origin).magnitude;
                        d2 = Vector3.Cross(ray.direction, hits[j + 1].collider.bounds.center - ray.origin).magnitude;
                        if (d1 > d2)
                        {
                            temp = hits[j + 1];
                            hits[j + 1] = hits[j];
                            hits[j] = temp;
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
                List<T> elementsANotInB = new List<T>();

                int counter = _A.Count;
                for (int i = 0; i < counter; i++)
                {
                    T element = _A[i];
                    if (!_B.Contains(element))
                    {
                        elementsANotInB.Add(element);
                    }
                }

                return elementsANotInB;
            }

            public static float GetClosestToNumberInList(float _num, float[] _list)
            {
                float toReturn = -1;
                float minDistance = float.MaxValue;
                float tmpDistance = float.MaxValue;
                for (int i = 0; i < _list.Length; ++i)
                {
                    tmpDistance = Mathf.Abs(_list[i] - _num);
                    if (tmpDistance < minDistance)
                    {
                        toReturn = _list[i];
                        minDistance = tmpDistance;
                    }
                }
                return toReturn;
            }
            /// <summary>
            /// Convert a number belonging to interval A to its correspective in interval B
            /// </summary>
            /// <param name="_ANum"></param>
            /// <param name="_AMin"></param>
            /// <param name="_AMax"></param>
            /// <param name="_BMin"></param>
            /// <param name="_BMax"></param>
            /// <returns></returns>

            public static float ConvertNumberFromIntevalAToB(float _ANum, float _AMin, float _AMax, float _BMin, float _BMax)
            {
                return _BMin + ((_ANum - _AMin) / (_AMax - _AMin)) * (_BMax - _BMin);
            }

            public static List<T> Shuffle<T>(List<T> list)
            {
                System.Random rnd = new System.Random();
                for (int i = 0; i < list.Count; i++)
                {
                    int k = rnd.Next(0, i);
                    T value = list[k];
                    list[k] = list[i];
                    list[i] = value;
                }
                return list;
            }
        }

        public static class ExtensionMethods
        {

            public static float Remap(this float value, float from1, float to1, float from2, float to2)
            {
                return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
            }
        }

        public class ReadOnlyAttribute : PropertyAttribute { }



#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
#endif
}
