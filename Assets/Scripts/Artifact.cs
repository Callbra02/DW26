using UnityEngine;

public class Artifact : MonoBehaviour
{
    public bool isHeld = false;
    public Sprite artifactSprite;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("ArtifactPlace"))
        {
            if (isHeld)
            {
                GameManager.Instance.objectivesCompleted++;
                this.transform.parent.GetComponent<bPlayerController>().DropArtifact();
                Destroy(this.gameObject);
            }
        }
    }
}
