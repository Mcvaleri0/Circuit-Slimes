using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]

public class Outline : MonoBehaviour {

  private static HashSet<Mesh> RegisteredMeshes = new HashSet<Mesh>();

    public bool Selected = false;

    private Color SelectedColor = Color.white;

    private bool needsUpdate;

    private class RendererBlock
    {
        public MaterialPropertyBlock block;
        public Color cachedColor = Color.magenta;
        public Renderer renderer;

        public RendererBlock(Renderer renderer, Color cachedColor)
        {
            this.block = new MaterialPropertyBlock();
            this.cachedColor = cachedColor;
            this.renderer = renderer;
        }
    }

    List<RendererBlock> RendererBlocks;

    public void Select()
    {
        this.Selected = true;
        needsUpdate = true;
    }

    public void DeSelect()
    {
        this.Selected = false;
        needsUpdate = true;
    }

    void Awake()
    {
        // Cache renderersblocks
        this.RendererBlocks = new List<RendererBlock>();

        //get all renderers to iterate through
        var allRenderers = GetComponentsInChildren<Renderer>();

        foreach (var renderer in allRenderers)
        {
            var materials = renderer.sharedMaterials.ToList();
            foreach (var mat in materials)
            {
                if (mat.HasProperty("_OutlineColor"))
                {
                    var col = mat.GetColor("_OutlineColor");
                    this.RendererBlocks.Add(new RendererBlock(renderer, col));
                }
            }
        }

        this.needsUpdate = true;
    }

    void Update()
    {
        if (needsUpdate)
        {
            needsUpdate = false;

            foreach(var rendererBlock in this.RendererBlocks)
            {
                UpdateMaterialProperties(rendererBlock);
            }
        }
    }

    void UpdateMaterialProperties(RendererBlock rendererBlock)
    {
        var block = rendererBlock.block;
        var renderer = rendererBlock.renderer;
        var cachedCol = rendererBlock.cachedColor;

        renderer.GetPropertyBlock(block);
        var col = (this.Selected)? this.SelectedColor : cachedCol;
        block.SetColor("_OutlineColor", col);

        renderer.SetPropertyBlock(block);
    }
}
