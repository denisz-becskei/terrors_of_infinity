using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static GenerationManager;

public class PlayerInformation : MonoBehaviour, IDataPersistance
{
    public ChunkType currentChunkType;
    private ChunkType previousChunkType = ChunkType.None;

    public bool walkVision = false;

    private RaycastHit hit;
    public string playerCurrentWorldPosition;
    [SerializeField] private StatusEffectController statusEffectController;
    [SerializeField] private OnChangeChunkType onChangeChunkType;

    [SerializeField] private DialogUIHandler dialog;

    [SerializeField] private TMP_Text scoreText;

    private Coroutine updatePositionRoutine;
    private Dictionary<ChunkType, bool> wasVisited = new Dictionary<ChunkType, bool>();
    private bool isDialogSystemRunning = false;

    private Dictionary<String, int> numberOfBits;

    public long currentScore = 0;

    private void Start()
    {
        StartCoroutine(LateStart());
        updatePositionRoutine = StartCoroutine(UpdatePlayerPosition());
    }

    public void ChunkUpdateAction()
    {
        if(previousChunkType != currentChunkType)
        {
            onChangeChunkType.StartAction(previousChunkType, currentChunkType);
            previousChunkType = currentChunkType;
        }

        // Turn off Player Position Updating when in Purgatory
        if(currentChunkType == ChunkType.Purgatory)
        {
            statusEffectController.ResetAllStatusEffects();
            StopCoroutine(updatePositionRoutine);
            updatePositionRoutine = null;
        } else { 
            updatePositionRoutine = StartCoroutine(UpdatePlayerPosition());
        }

        if(isDialogSystemRunning)
        {
            DialogUpdateAction();
        }
    }

    private IEnumerator UpdatePlayerPosition()
    {
        yield return new WaitForSeconds(2f);
        PositionUpdate();
    }

    private void PositionUpdate()
    {
        Vector3 newRaycastPosition = transform.position + Vector3.up * 3f;

        if (Physics.Raycast(newRaycastPosition, Vector3.down, out hit, 20f) && hit.transform.CompareTag("Ground"))
        {
            RoomPosition rp = hit.transform.parent?.GetComponent<RoomPosition>();
            if (rp != null)
            {
                playerCurrentWorldPosition = rp.GetRoomCoordinatesInWorld();
            }
        }

        updatePositionRoutine = StartCoroutine(UpdatePlayerPosition());
    }

    public void LoadData(GameStates data)
    {
        playerCurrentWorldPosition = data.PLAYER_WORLD_COORDINATES;
        numberOfBits = data.PICKED_UP_BITS;
        currentScore = data.PLAYER_SCORE;
        Debug.Log("Loading World Position");
        // TODO: Move player there
    }

    public void SaveData(ref GameStates data)
    {
        data.PLAYER_WORLD_COORDINATES = playerCurrentWorldPosition;
        data.PICKED_UP_BITS = this.numberOfBits;
        data.PLAYER_SCORE = this.currentScore;
        Debug.Log("Saving World Position");
    }

    public void UpdateScore() {
        this.scoreText.text = currentScore.ToString();
    }

    private void DialogUpdateAction()
    {
        if(wasVisited.Count == 0)
        {
            PopulateVisitedDictionary();
        }

        if(currentChunkType == ChunkType.Purgatory)
        {
            return;
        } else if (wasVisited[currentChunkType] == false)
        {
            bool success = dialog.Play(WorldWideScripts.chunkTypesByInt[((int)currentChunkType)], false);
            if (success)
            {
                wasVisited[currentChunkType] = true;
            }
        }
    }

    private void PopulateVisitedDictionary()
    {
        for(int i = 1; i < 19; i++)
        {
            wasVisited.Add((ChunkType)i, false);
        }
    }

    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(5f);
        isDialogSystemRunning = true;
    }

    public void PickedUpBitOfColor(String color)
    {
        this.numberOfBits[color] += 1;
    }
}
