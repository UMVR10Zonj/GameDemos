using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileDisappearing : MonoBehaviour
{
    public Tilemap tilemap;
    public float disappearTime = 2f;
    public ParticleSystem vfx;

    private void Init()
    {
        vfx.Pause();
    }
    private void Start()
    {
        Init();
        ObjectPool.SetupNewPool(ObjectPool.EType.FX_MFRoundBlue, vfx.gameObject, 20);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (other.transform.tag != "Player") return;

        Vector3Int cellPos = tilemap.WorldToCell(other.transform.position);
        cellPos.y = (Physics2D.gravity.y < 0) ? cellPos.y - 1 : cellPos.y + 1;

        TileBase oriTile = tilemap.GetTile(cellPos);

        if (oriTile == null) return;
        if (oriTile.name != "GhostSquare") return;

        StartCoroutine(SetNull(cellPos, oriTile));
    }
    private IEnumerator SetNull(Vector3Int cellPos, TileBase oriTile)
    {
        float time = 0;
        while (time < 0.1f)
        {
            time += Time.deltaTime;
            yield return null;
        }

        tilemap.SetTile(cellPos, null);

        GameObject Vfx = ObjectPool.GetObject(ObjectPool.EType.FX_MFRoundBlue);
        Vector3 Vpos = cellPos;
        Vpos.x += 0.5f;
        Vpos.y += 0.5f;
        Vfx.transform.position = Vpos;
        Vfx.SetActive(true);

        ParticleSystem p = Vfx.GetComponent<ParticleSystem>();
        while (p.isPlaying)
        {
            yield return null;
        }
        Vfx.SetActive(false);
        ObjectPool.ReleaseObject(Vfx);

        yield return new WaitForSeconds(disappearTime);

        tilemap.SetTile(cellPos, oriTile);
    }
}
