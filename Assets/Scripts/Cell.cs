using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private bool hasMine = false;
    public bool HasMine
    {
        get { return hasMine; }
        set
        {
            if (!hasMine) hasMine = value;
        }
    }
    [SerializeField] private bool hasFlag = false;
    [SerializeField] private bool isOpened = false;
    private Renderer renderer;
    public Field Field { get; set; }
    public int XIndex { get; set; }
    public int YIndex { get; set; }

    public static event Action<Cell> OnBlowUp;

    // Start is called before the first frame update
    void Start()
    {
        OnBlowUp += OnAnyBlowUp;
        renderer = GetComponent<Renderer>();
    }

    private void CheckAround()
    {
        int minesAround = 0;
        for (int i = XIndex - 1;  i <= XIndex + 1; i++) 
        {
            for(int j = YIndex - 1; j <= YIndex + 1; j++)
            {
                minesAround += Convert.ToInt32(Field.GetCellAtCoordinate(i, j)?.HasMine ?? false);
            }
        }
        if (minesAround == 0)
        {
            for (int i = XIndex - 1; i <= XIndex + 1; i++)
            {
                for (int j = YIndex - 1; j <= YIndex + 1; j++)
                {
                    Cell cellToOpen = Field.GetCellAtCoordinate(i, j);
                    if (cellToOpen == null || (i == XIndex && j == YIndex))
                        continue;
                    cellToOpen.Open();
                }
            }
            renderer.material.mainTexture = Resources.Load($"CellTextures/CellEmptyTexture") as Texture2D;
        }
        else
        {
            renderer.material.mainTexture = Resources.Load($"CellTextures/Cell{minesAround}Texture") as Texture2D;
        }      
    }

    private void Open()
    {
        if (hasFlag || isOpened) return;
        isOpened = true;
        if (hasMine)
        {
            OnBlowUp.Invoke(this);
            return;
        }
        CheckAround();
        return;
    }

    public void ChangeFlag()
    {
        hasFlag = !hasFlag;
        //TODO: Switch cell texture
    }

    public void SelfBlowUp()
    {
        //TODO: Explosion animation and particle systems
    }

    public void OnAnyBlowUp(Cell invoker)
    {
        if (!hasFlag && hasMine) SelfBlowUp(); 
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Open();
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ChangeFlag();
        }
    }

}
