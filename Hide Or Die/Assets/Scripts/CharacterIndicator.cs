using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterIndicator : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [SerializeField] private Sprite redTeamIndicatorSprite;
    [SerializeField] private Sprite blueTeamIndicatorSprite;

    private void Start()
    {
        spriteRenderer = this.gameObject.GetComponent<SpriteRenderer>();

        if (this.transform.parent.CompareTag("BlueTeam"))
            spriteRenderer.sprite = blueTeamIndicatorSprite;
        else
            spriteRenderer.sprite = redTeamIndicatorSprite;
    }
}
