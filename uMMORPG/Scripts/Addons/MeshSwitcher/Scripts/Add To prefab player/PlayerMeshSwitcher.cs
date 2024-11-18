//using Mirror;
using System;
using UnityEngine;

// PLAYER
public class PlayerMeshSwitcher : MonoBehaviour// NetworkBehaviour
{
#if _iMMOMESHSWITCHER
    public Player player;
    public PlayerEquipment playerEquipment;

    protected Renderer[] cachedRenderers;


    // -----------------------------------------------------------------------------------
    // OnStartClient_Tools_MeshSwitcher -> to Start
    // -----------------------------------------------------------------------------------
    private void Start()
    {
        EquipmentInfo[] infoList = ((PlayerEquipment)player.equipment).slotInfo;
        for (int index = 0; index < infoList.Length; ++index)
        {
            EquipmentInfo info = infoList[index];
            for (int i = 0; i < info.mesh.Length; ++i)
            {
                if (info.mesh[i].mesh != null)
                {
                    //Debug.LogError("OH");
                    info.mesh[i].defaultMaterial = info.mesh[i].mesh.GetComponent<Renderer>().material;
                }
            }
            RefreshMesh(index);
        }
        playerEquipment.onRefreshLocation.AddListener(OnRefreshLocation_MeshSwitcher);
    }

    // -----------------------------------------------------------------------------------
    // OnRefreshLocation_MeshSwitcher
    // -----------------------------------------------------------------------------------
    public void OnRefreshLocation_MeshSwitcher(int index)
    {
        RefreshMesh(index);
    }

    // -----------------------------------------------------------------------------------
    // RefreshMesh
    // -----------------------------------------------------------------------------------
    private void RefreshMesh(int index)
    {
        //Debug.LogError("REFRESHHH!!!");
        ItemSlot slot = player.equipment.slots[index];
        EquipmentInfo info = ((PlayerEquipment)player.equipment).slotInfo[index];
        cachedRenderers = new Renderer[info.mesh.Length];

        for (int i = 0; i < info.mesh.Length; ++i)
        {
            cachedRenderers[i] = info.mesh[i].mesh.GetComponent<Renderer>();
        }

        if (info.requiredCategory != "" && info.mesh.Length > 0)
        {
            //Debug.Log("JE passe ici");
            if (slot.amount > 0)
            {
                EquipmentItem itemData = (EquipmentItem)slot.item.data;
                SwitchToMesh(info, itemData.meshIndex, itemData.meshMaterial, itemData.switchableColors);
            }
            else
            {
                SwitchToMesh(info, 0, null, null);
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // SwitchToMesh
    // -----------------------------------------------------------------------------------
    private void SwitchToMesh(EquipmentInfo info, int[] meshIndex, Material mat, SwitchableColor[] colors)
    {
        for (int i = 0; i < info.mesh.Length; ++i)
        {
            if (info.mesh[i].mesh != null)
            {
                if (Array.Exists(meshIndex, e => e == i))
                {
                    info.mesh[i].mesh.SetActive(true);

                    if (mat != null)
                    {
                        cachedRenderers[i].material = mat;
                    }
                    else if (info.mesh[i].defaultMaterial != null)
                    {
                        cachedRenderers[i].material = info.mesh[i].defaultMaterial;
                    }

                    SwitchToColor(cachedRenderers[i].material, colors);
                }
                else
                {
                    info.mesh[i].mesh.SetActive(false);
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // SwitchToMesh
    // -----------------------------------------------------------------------------------
    private void SwitchToMesh(EquipmentInfo info, int meshIndex, Material mat, SwitchableColor[] colors)
    {
        for (int i = 0; i < info.mesh.Length; ++i)
        {
            if (info.mesh[i].mesh != null)
            {
                if (meshIndex == i)
                {
                    info.mesh[i].mesh.SetActive(true);

                    if (mat != null)
                    {
                        info.mesh[i].mesh.GetComponent<Renderer>().material = mat;
                    }
                    else if (info.mesh[i].defaultMaterial != null)
                    {
                        info.mesh[i].mesh.GetComponent<Renderer>().material = info.mesh[i].defaultMaterial;
                    }

                    SwitchToColor(cachedRenderers[i].material, colors);
                }
                else
                {
                    info.mesh[i].mesh.SetActive(false);
                }
            }
        }
    }

    // -----------------------------------------------------------------------------------
    // SwitchToColor
    // -----------------------------------------------------------------------------------
    private void SwitchToColor(Material material, SwitchableColor[] colors)
    {

        if (material == null || colors == null || colors.Length == 0) return;
        foreach (SwitchableColor color in colors)
        {
            material.SetColor(color.propertyName.ToString(), color.color);
           ///   Debug.LogError("switch color...? " + color.ToString());
        }
    }
#else
    [ReadOnly] public string enableMEshSwitcher = "Mesh Switcher is not enabled";
#endif
    // -----------------------------------------------------------------------------------
}
