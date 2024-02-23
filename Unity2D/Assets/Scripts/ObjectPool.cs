using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public enum EType { FX_Rocket, FX_MFRoundBlue }
    public class ObjectData
    {
        public EType oType;
        public GameObject go;

        public bool inUse;
    }

    #region Singleton

    private static ObjectPool Instance = null;
    private ObjectPool() { }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #endregion

    private Dictionary<EType, Queue<ObjectData>> _pool = new Dictionary<EType, Queue<ObjectData>>();
    private List<ObjectData> _inUseList = new List<ObjectData>();

    public static void SetupNewPool(EType t, Object o, int iCount) => Instance.onSetupNewPool(t, o, iCount);
    public static GameObject GetObject(EType e) => Instance.onGetObject(e);
    public static void ReleaseObject(GameObject go) => Instance.onReleaseObject(go);

    private void onSetupNewPool(EType t, Object o, int iCount)
    {
        Queue<ObjectData> que = new Queue<ObjectData>();
        GameObject parent = new GameObject(o.name);
        parent.transform.parent = transform;

        for (int i = 0; i < iCount; i++)
        {
            GameObject go = Instantiate(o, parent.transform) as GameObject;

            ObjectData oData = new ObjectData();
            oData.oType = t;
            oData.go = go;
            oData.inUse = false;

            oData.go.SetActive(false);

            que.Enqueue(oData);
        }

        _pool[t] = que;
    }
    private GameObject onGetObject(EType e)
    {
        if (_pool[e].Count > 0)
        {
            var oData = _pool[e].Dequeue();
            oData.inUse = true;
            _inUseList.Add(oData);
            return oData.go;
        }
        else
        {
            return null;
        }
    }
    private void onReleaseObject(GameObject go)
    {
        for (int i = 0; i < _inUseList.Count; i++)
        {
            if (_inUseList[i].go == go)
            {
                _inUseList[i].inUse = false;
                _pool[_inUseList[i].oType].Enqueue(_inUseList[i]);
                _inUseList.Remove(_inUseList[i]);
            }
        }
    }
}
