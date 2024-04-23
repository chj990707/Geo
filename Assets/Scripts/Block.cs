using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Block : MonoBehaviour
{
    private bool initialized = false;
    [SerializeField]
    private bool opaque;
    [SerializeField]
    private bool fullBlock;
    public GridManager gridManager { get; protected set; }
    public int x { get; protected set; }
    public int y { get; protected set; }
    public int z { get; protected set; } 

    public void Initialize(GridManager gridManager, int x, int y, int z)
    {
        if(initialized) return;
        initialized = true;
        this.gridManager = gridManager;
        this.x = x;
        this.y = y;
        this.z = z;
        transform.localPosition = gridManager.getGrid().CellToWorld(new Vector3Int(x, y, z));
    }

    public virtual Block[] getAdjacentBlocks()
    {
        Block[] adajcentBlocks = new Block[6]; 
        adajcentBlocks[0] = gridManager.getBlock(x - 1, y, z);
        adajcentBlocks[1] = gridManager.getBlock(x + 1, y, z);
        adajcentBlocks[2] = gridManager.getBlock(x, y - 1, z);
        adajcentBlocks[3] = gridManager.getBlock(x, y + 1, z);
        adajcentBlocks[4] = gridManager.getBlock(x, y, z + 1);
        adajcentBlocks[5] = gridManager.getBlock(x, y, z - 1);
        return adajcentBlocks;
    }

    public virtual void UpdateBlock()
    {
        if (isVisible())
        {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
        else 
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        if (isCollidable())
        {
            gameObject.GetComponent<Collider>().enabled = true;
        }
        else
        {
            gameObject.GetComponent<Collider>().enabled = false;
        }
    }
    
    public virtual bool isVisible()
    {
        Block[] adjacentBlocks = getAdjacentBlocks();
        foreach (Block adjacentBlock in adjacentBlocks)
        {
            if (adjacentBlock == null || !adjacentBlock.opaque || !adjacentBlock.fullBlock) return true;
        }
        return false;
    }
    public virtual bool isCollidable()
    {
        Block[] adjacentBlocks = getAdjacentBlocks();
        foreach (Block adjacentBlock in adjacentBlocks)
        {
            if (adjacentBlock == null || !adjacentBlock.fullBlock) return true;
        }
        return false;
    }
}
