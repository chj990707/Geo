using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CrackableBlock : Block
{
    abstract protected float hardness { get; }
    protected Vector3[] crackDirections = new Vector3[]
        { new Vector3(1, 0, 0),
        new Vector3(-1, 0, 0),
        new Vector3(0, 1, 0),
        new Vector3(0, -1, 0),
        new Vector3(0, 0, 1),
        new Vector3(0, 0, -1) };
    [SerializeField]
    private GameObject crackResult;

    public virtual void crackPropagate(float force, Vector3 forceFrom)
    {
        if (hardness < force)
        {
            float remainingforce = force - hardness;
            Vector3 normalizedforceFrom = Vector3.Normalize(forceFrom);
            float[] crackforce = new float[crackDirections.Length];
            float crackforcesum = 0f;
            for(int i = 0; i < crackDirections.Length; i++)
            {
                Vector3 nextCrackDirection = crackDirections[i];
                Block nextBlock = gridManager.getBlock(x + (int)nextCrackDirection.x, y + (int)nextCrackDirection.y, z + (int)nextCrackDirection.z);
                
                if (Vector3.Dot(nextCrackDirection, normalizedforceFrom) >= 0.5f)
                {
                    if (nextBlock != null && nextBlock is CrackableBlock)
                    {
                        crackforce[i] += 10f;
                        crackforcesum += 10f;
                    }
                    else
                    {
                        crackforce[i] += 5f;
                        crackforcesum += 5f;
                    }
                }
                else if (Vector3.Dot(nextCrackDirection, normalizedforceFrom) <= -0.5f) continue;
                else
                {
                    if (nextBlock != null && nextBlock is CrackableBlock)
                    {
                        if(Random.Range(0f, 1f) > ((CrackableBlock)nextBlock).hardness / remainingforce)
                        {
                            crackforce[i] += 5f;
                            crackforcesum += 5f;
                        }
                    }
                    else
                    {
                        crackforce[i] += 2.5f;
                        crackforcesum += 2.5f;
                    }
                }
            }
            for (int i = 0; i < crackDirections.Length; i++)
            {
                Vector3 nextCrackDirection = crackDirections[i];
                Block nextBlock = gridManager.getBlock(x + (int)nextCrackDirection.x, y + (int)nextCrackDirection.y, z + (int)nextCrackDirection.z);
                if(nextBlock != null && nextBlock is CrackableBlock)
                {
                    ((CrackableBlock)nextBlock).crackPropagate(remainingforce * crackforce[i] / crackforcesum, nextCrackDirection);
                }
            }
            Block[] adjacentBlocks = getAdjacentBlocks();
            gridManager.removeBlock(x, y, z);
            if (crackResult != null)
            {
                GameObject afterCrackedBlock = GameObject.Instantiate(crackResult);
                gridManager.setBlock(x, y, z, afterCrackedBlock.GetComponent<Block>());
                afterCrackedBlock.GetComponent<Block>().Initialize(gridManager, x, y, z);
                afterCrackedBlock.GetComponent<Block>().UpdateBlock();
            }
        }
    }
}
