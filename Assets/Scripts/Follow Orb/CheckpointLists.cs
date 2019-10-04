using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointLists : MonoBehaviour
{
    [SerializeField, Tooltip("The list of lists of checkpoints that the follow orb will pick between.")]
    public List<ListWrapper> CheckpointListsList = new List<ListWrapper>();
}
