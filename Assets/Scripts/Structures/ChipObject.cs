using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using static UnityEngine.EventSystems.EventTrigger;

[CreateAssetMenu(fileName = "ChipObject", menuName = "ScriptableObjects/ChipObject")]
public class ChipObject : ScriptableObject
{
    public int chipPrice;
    public AssetReferenceGameObject chip2DPrefabAsset;
    public AssetReferenceGameObject chip3DPrefabAsset;
    public GameObject Chip2DPrefab { get;private set;}
    public GameObject Chip3DPrefab { get;private set;}
    public Entry EventEntry { get; private set; }
    public Bet ChipBet { get; set; }
    public async Task LoadChipAssets()
    {
        Chip2DPrefab = await ObjectManager.GetObject(chip2DPrefabAsset);
        Chip3DPrefab = await ObjectManager.GetObject(chip3DPrefabAsset);
        Chip2DPrefab.gameObject.SetActive(false);
        Chip3DPrefab.gameObject.SetActive(false);
        EventEntry = Chip2DPrefab.GetComponent<EventTrigger>().triggers[0];
    }
    public void SetPositionAndRotation(Vector3 position,Vector3 rotation)
    {
        Chip3DPrefab.transform.position = position;
        Chip3DPrefab.transform.eulerAngles = rotation;
    }
    public void Chip3DSetActive(bool isActive)
    {
        Chip3DPrefab.gameObject.SetActive(isActive);
    }
    public void DisposeBet() => ChipBet = null;
    public void DisposeChip()
    {
        ChipBet = null;
        Chip3DPrefab.gameObject.SetActive(false);
    }
}
