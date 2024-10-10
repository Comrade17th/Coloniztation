using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ResourcesDataBase : MonoBehaviour
{
   private HashSet<Resource> _resources = new();

   public void ReserveResource(Resource resource)
   {
      _resources.Add(resource);
   }

   public IEnumerable<Resource> GetFreeResources(IEnumerable<Resource> resources)
   {
      return resources.Where(resource => _resources.Contains(resource) == false);
   }

   public void RemoveReservation(Resource resource)
   {
      _resources.Remove(resource);
   }
}
