using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using TMPro;

namespace Architecture
{
    [DisallowMultipleComponent, SelectionBase]
    public class BoxInteractable : InteractableBase
    {
        [SerializeField] SpriteRenderer spriteRend;
        private int blocksStored;
        [SerializeField] BuildingSystem.BlockType storedBlockType;
        [SerializeField] GameObject blockInstance;
        [SerializeField] TextMeshProUGUI storageText;

        private void Start()
        {
            if (storedBlockType == BuildingSystem.BlockType.WOOD)
            {
                objectType = ObjectType.WOODBOX;
                blocksStored = 25;
            }
            if (storedBlockType == BuildingSystem.BlockType.METAL)
            {
                objectType = ObjectType.METALBOX;
                blocksStored = 8;
            }
        }

        private void Update()
        {
            storageText.text = blocksStored.ToString();
        }

        public override void Interact()
        {
            spriteRend.color = Color.green;
        }

        public override void LeaveInteract()
        {

        }

        #region PUBLIC_INTERFACE
        public int GetBlocksStored()
        {
            return blocksStored;
        }

        public BuildingSystem.BlockType GetBlockType()
        {
            return storedBlockType;
        }

        public void DecreaseBlocksStored(int n)
        {
            if (blocksStored - n < 0)
            {
                blocksStored = 0;
            }
            else
            {
                blocksStored -= n;
            }
        }

        public bool IsEmpty()
        {
            return blocksStored == 0;
        }

        public GameObject GetBlockInstance()
        {
            return blockInstance;
        }
        #endregion
    }
}