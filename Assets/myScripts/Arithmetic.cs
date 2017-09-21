using System.Collections;
using UnityEngine;

namespace Assets.myScripts
{
    class Arithmetic
    {
        public IEnumerator getSpeedOfObject(GameObject obj, int checksPerSecond)
        {
            // Save current position
            Vector3 savedPosition = obj.transform.position;

            // Wait
            yield return new WaitForSeconds(1f / checksPerSecond);

            // Calculate and return speed using distance traveled
            yield return Vector3.Distance(savedPosition, obj.transform.position) / (1f / checksPerSecond);
        }
    }
}
