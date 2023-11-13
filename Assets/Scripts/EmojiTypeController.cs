using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmojiTypeController : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer emojiRend;
    private EmojiType currEmojiType = EmojiType.Happy;
    private bool inFastSpining = false;
    private bool touchedGround = false;
    private float fastSpinTimer = 0.0f;
    private float highestY = 0.0f;

    private void Update()
    {
        if (transform.position.y > highestY)
        {
            highestY = transform.position.y;
        }

        if (inFastSpining)
        {
            fastSpinTimer += Time.deltaTime;
        }
    }

    public void SetWaterEmoji(bool intoWater)
    {
        currEmojiType = intoWater ? EmojiType.Dead : EmojiType.Suprised;
        SetEmojiSprite();
    }

    public void SetJumpEmoji()
    {
        currEmojiType = EmojiType.Shocked;
        SetEmojiSprite();
    }

    public void SetFastSpin()
    {
        if (!inFastSpining) { return; }
        if (fastSpinTimer < GameManager.instance.GameScriptObj.BarrelEmojiFastSpinDizzyTime)
        {
            currEmojiType = EmojiType.Dizzy;
        }
        else if (fastSpinTimer < GameManager.instance.GameScriptObj.BarrelEmojiFastSpinDisgustedTime)
        {
            currEmojiType = EmojiType.Disgusted;
        }
        else
        {
            currEmojiType = EmojiType.Puked;
        }
        SetEmojiSprite();
    }

    public void StartFastSpining(bool tf)
    {
        if (inFastSpining == tf) { return; }
        inFastSpining = tf;
        fastSpinTimer = 0.0f;
    }

    public void FallBackDown()
    {
        if (Mathf.Abs(highestY - transform.position.y) > GameManager.instance.GameScriptObj.BarrelEmojiFallBackThreshold)
        {
            int typeNum = Random.Range(0, 4);
            switch (typeNum)
            {
                case 0:
                    currEmojiType = EmojiType.Cry;
                    break;
                case 1:
                    currEmojiType = EmojiType.Angry;
                    break;
                case 2:
                    currEmojiType = EmojiType.Hate;
                    break;
                case 3:
                    currEmojiType = EmojiType.Unhappy;
                    break;
                default:
                    Debug.Log("ERROR: Undefined Emoji Type For Fall Down");
                    break;
            }
            inFastSpining = false;
            SetEmojiSprite();
        }
        highestY = transform.position.y;
    }

    public void SetClimbing()
    {
        if (!touchedGround) { return; }
        currEmojiType = EmojiType.Sorry;
        SetEmojiSprite();
    }

    public void SetSmashWall()
    {
        currEmojiType = EmojiType.Unsettle;
        SetEmojiSprite();
        Invoke(nameof(SetInjured), GameManager.instance.GameScriptObj.BarrelEmojiInjuredTime);
    }

    public void SetInjured()
    {
        currEmojiType = EmojiType.Injured;
        SetEmojiSprite();
    }

    public void SetStand()
    {
        currEmojiType = EmojiType.Force;
        SetEmojiSprite();

    }

    public void SetLook()
    {
        currEmojiType = EmojiType.OhWhat;
        SetEmojiSprite();
    }

    public void SetTouchGround(bool tf)
    {
        touchedGround = tf;
    }

    public void SetNormal()
    {
        int emojiIndex = Random.Range(0, 3);
        switch (emojiIndex)
        {
            case 0:
                currEmojiType = EmojiType.Happy;
                break;
            case 1:
                currEmojiType = EmojiType.Excited;
                break;
            case 2:
                currEmojiType = EmojiType.Huh;
                break;
            default:
                Debug.Log("ERROR: Undefined Emoji Type For Normal");
                break;
        }
        SetEmojiSprite();
    }

    private void SetEmojiSprite()
    {
        emojiRend.sprite = GameManager.instance.GameScriptObj.EmojiTypes[(int)currEmojiType];
    }
}
