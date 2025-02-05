using UnityEngine;

public class TagSpot : MonoBehaviour
{
    public string ExpectedTag { get; private set; }
    public bool IsPreFilled { get; private set; }
    public bool IsOccupied { get; private set; }
    public TagObject CurrentTag { get; private set; }

    public bool IsCorrectlyPlaced => IsOccupied && CurrentTag != null &&
                                   CurrentTag.GetTagText() == ExpectedTag;

    public void Initialize(string expectedTag, bool isPrefilled)
    {
        ExpectedTag = expectedTag;
        IsPreFilled = isPrefilled;
        IsOccupied = isPrefilled;
    }

    public bool PlaceTag(TagObject tag)
    {
        if (tag.GetTagText() != ExpectedTag) return false;

        IsOccupied = true;
        CurrentTag = tag;
        tag.transform.position = transform.position + Vector3.up * 0.2f;
        tag.transform.rotation = transform.rotation;

        if (TagParticleSystem.Instance != null)
            TagParticleSystem.Instance.PlaySuccessEffect(transform.position);

        if (GameManager.Instance.TagManager.CheckLevelCompletion())
        {
            Debug.Log($"VICTORY! Level completed!");
            EventSystem.RaiseLevelCompleted();
        }

        return true;
    }

    public void ClearTag()
    {
        IsOccupied = false;
        CurrentTag = null;
    }
}