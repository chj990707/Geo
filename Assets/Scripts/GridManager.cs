using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField]
    private Grid grid;
    protected Block[,,] blocks = new Block[12, 12, 12];
    [SerializeField]
    protected GameObject initBlock;

    public Grid getGrid()
    {
        return grid;
    }

    public void createRock()
    {
        for (int i = 0; i < blocks.GetLength(0); i++)
        {
            for (int j = 0; j < blocks.GetLength(1); j++)
            {
                for (int k = 0; k < blocks.GetLength(2); k++)
                {
                    if (blocks[i, j, k] != null)
                    {
                        Destroy(blocks[i, j, k].gameObject);
                    }
                }
            }
        }
        for (int i = 0; i < blocks.GetLength(0); i++)
        {
            for (int j = 0; j < blocks.GetLength(1); j++)
            {
                for (int k = 0; k < blocks.GetLength(2); k++)
                {
                    GameObject newBlock = GameObject.Instantiate(initBlock);
                    newBlock.GetComponent<Block>().Initialize(this, i, j, k);
                    blocks[i, j, k] = newBlock.GetComponent<Block>();
                }
            }
        }

        for (int i = 0; i < blocks.GetLength(0); i++)
        {
            for (int j = 0; j < blocks.GetLength(1); j++)
            {
                for (int k = 0; k < blocks.GetLength(2); k++)
                {
                    if (blocks[i, j, k] != null)
                    {
                        blocks[i, j, k].UpdateBlock();
                    }
                }
            }
        }
        ((Stone)blocks[6, 11, 6]).crackPropagate(200f, new Vector3(0, -1, 0));
    }

    public void setBlock(int x, int y, int z, Block block)
    {
        blocks[x, y, z] = block;
    }

    public void removeBlock(int x, int y, int z)
    {
        Destroy(blocks[x, y, z].gameObject);
        blocks[x, y, z] = null;
    }
    public Block getBlock(int x, int y, int z)
    {
        if (x < 0 || x >= blocks.GetLength(0)
            || y < 0 || y >= blocks.GetLength(1)
            || z < 0 || z >= blocks.GetLength(2))
        {
            return null;
        }
        return blocks[x, y, z];
    }
}
