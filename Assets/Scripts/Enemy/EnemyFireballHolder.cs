using UnityEngine;
public class EnemyFireballHolder : MonoBehaviour
{
    [SerializeField] private Transform enmey;
    private void Update()
    {
        transform.localScale=enmey.localScale;
    }
}