using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmojiTypeController : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer emojiRend;
    private bool inFastSpining = false;
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
            SetFastSpin();
        }
    }

    public void SetWaterEmoji(bool intoWater)
    {
        SetEmojiSprite(intoWater ? EmojiType.Dead : EmojiType.Suprised);
    }

    public void SetJumpEmoji()
    {
        SetEmojiSprite(EmojiType.Shocked);
    }

    public void SetFastSpin()
    {
        if (fastSpinTimer < GameManager.instance.GameScriptObj.BarrelEmojiFastSpinDizzyTime)
        {
            SetEmojiSprite(EmojiType.Dizzy);
        }
        else if (fastSpinTimer < GameManager.instance.GameScriptObj.BarrelEmojiFastSpinDisgustedTime)
        {
            SetEmojiSprite(EmojiType.Disgusted);
        }
        else
        {
            SetEmojiSprite(EmojiType.Puked);
        }
    }

    public void StartFastSpining(bool tf)
    {
        if (inFastSpining == tf) { return; }
        inFastSpining = tf;
        fastSpinTimer = 0.0f;
        if (!tf)
        {
            //SetNormal();
        }
    }

    public void FallBackDown()
    {
        if (Mathf.Abs(highestY - transform.position.y) > GameManager.instance.GameScriptObj.BarrelEmojiFallBackThreshold)
        {
            int typeNum = Random.Range(0, 4);
            switch (typeNum)
            {
                case 0:
                    SetEmojiSprite(EmojiType.Cry);
                    break;
                case 1:
                    SetEmojiSprite(EmojiType.Angry);
                    break;
                case 2:
                    SetEmojiSprite(EmojiType.Hate);
                    break;
                case 3:
                    SetEmojiSprite(EmojiType.Unhappy);
                    break;
                default:
                    Debug.Log("ERROR: Undefined Emoji Type For Fall Down");
                    break;
            }
            inFastSpining = false;
        }
        highestY = transform.position.y;
    }

    public void SetClimbing()
    {
        SetEmojiSprite(EmojiType.Sorry);
    }

    public void SetSmashWall()
    {
        SetEmojiSprite(EmojiType.Unsettle);
        Invoke(nameof(SetInjured), GameManager.instance.GameScriptObj.BarrelEmojiInjuredTime);
    }

    public void SetInjured()
    {
        SetEmojiSprite(EmojiType.Injured);
    }

    public void SetStand()
    {
        SetEmojiSprite(EmojiType.Force);
    }

    public void SetLook()
    {
        SetEmojiSprite(EmojiType.OhWhat);
    }

    public void SetNormal()
    {
        int emojiIndex = Random.Range(0, 3);
        switch (emojiIndex)
        {
            case 0:
                SetEmojiSprite(EmojiType.Happy);
                break;
            case 1:
                SetEmojiSprite(EmojiType.Excited);
                break;
            case 2:
                SetEmojiSprite(EmojiType.Huh);
                break;
            default:
                Debug.Log("ERROR: Undefined Emoji Type For Normal");
                break;
        }
    }

    private void SetEmojiSprite(EmojiType type)
    {
        CancelInvoke(nameof(SetNormal));
        Invoke(nameof(SetNormal), GameManager.instance.GameScriptObj.BarrelEmojiBackNormalTime);
        emojiRend.sprite = GameManager.instance.GameScriptObj.EmojiTypes[(int)type];
    }
}
