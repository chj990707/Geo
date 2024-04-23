using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CrackableBlock : Block
{
    abstract protected float hardness { get; }
    protected Vector3Int[] crackDirections = new Vector3Int[]
        { new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(0, 0, 1),
        new Vector3Int(0, 0, -1) };
    [SerializeField]
    private GameObject crackResult;

    protected class CrackStress {
        public CrackableBlock block;
        public float force;
        public Vector3 forceDirection;

        public CrackStress(CrackableBlock block, float force, Vector3 forceDirection)
        {
            this.block = block;
            this.force = force;
            this.forceDirection = forceDirection;
        }
    }

    public virtual void crackStart(float force, Vector3 forceDirection)
    {
        Queue<CrackStress> CrackStressQueue = new Queue<CrackStress>();
        CrackStressQueue.Enqueue(new CrackStress(this, force, forceDirection));
        while(CrackStressQueue.Count > 0)
        {
            CrackStress nextCrackStress = CrackStressQueue.Dequeue();
            nextCrackStress.block.crackPropagate(nextCrackStress.force, nextCrackStress.forceDirection, CrackStressQueue);
        }
    }

    protected virtual void crackPropagate(float force, Vector3 forceDirection, Queue<CrackStress> CrackStressQueue)
    {
        if (hardness < force)
        {
            float remainingforce = force - hardness;
            Vector3 normalizedforceDirection = Vector3.Normalize(forceDirection);
            float[] crackforce = new float[crackDirections.Length];
            float crackforcesum = 0f;
            for(int i = 0; i < crackDirections.Length; i++)
            {
                Vector3Int nextCrackDirection = crackDirections[i];
                Block nextBlock = gridManager.getBlock(x + nextCrackDirection.x, y + nextCrackDirection.y, z + nextCrackDirection.z);
                
                if (Vector3.Dot(nextCrackDirection, normalizedforceDirection) >= 0.5f)
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
                else if (Vector3.Dot(nextCrackDirection, normalizedforceDirection) <= -0.5f) continue;
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
                    CrackStressQueue.Enqueue(new CrackStress((CrackableBlock)nextBlock, remainingforce * crackforce[i] / crackforcesum, nextCrackDirection));
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
