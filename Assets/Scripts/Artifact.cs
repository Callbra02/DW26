using UnityEngine;

public class Artifact : MonoBehaviour
{
    public bool isHeld = false;
    public Sprite artifactSprite;
    public GameObject visualizer;


    private void Update()
    {
        if (visualizer == null)
        {
            return;
        }
        visualizer.transform.position = this.transform.position + new Vector3(57.6f, 0, 0);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("ArtifactPlace"))
        {
            if (isHeld)
            {
                GameManager.Instance.objectivesCompleted++;
                Destroy(visualizer.gameObject);
                this.transform.parent.GetComponent<bPlayerController>().DropArtifact();
                Destroy(this.gameObject);
            }
        }
    }
}
