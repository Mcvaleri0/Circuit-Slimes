using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Puzzle.Data
{
    [System.Serializable]
    public class ResourceData
    {
        public string Name;
        public int    Amount;


        public ResourceData(Resource resource)
        {
            this.Name   = resource.Prefab;
            this.Amount = resource.Amount;
        }


        public Resource CreateResource()
        {
            return new Resource(this.Name, this.Amount);
        }
    }

}
