using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmojiTypeController : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer emojiRend;
    [SerializeField]
    private SoundsPlayer emojiPlayer;
    private bool inFastSpining = false;
    private bool inWater = false;
    private bool isClimbing = false;
    private bool isEmojiTypeCoolDowned = true;
    private float fastSpinTimer = 0.0f;

    [SerializeField]
    private ClipsCollection[] audioClipsCollection;

    [System.Serializable]
    public class ClipsCollection
    {
        public AudioClip[] audioClips;
    }

    private void Update()
    {
        if (transform.position.y > GameManager.instance.GameCon.barrelHighestY)
        {
            GameManager.instance.GameCon.barrelHighestY = transform.position.y;
        }

        if (inFastSpining)
        {
            fastSpinTimer += Time.deltaTime;
            SetFastSpin();
        }
    }

    public void SetWaterEmoji(bool intoWater)
    {
        if (inWater == intoWater) { return; }
        inWater = intoWater;
        SetEmojiSprite(intoWater ? EmojiType.Dead : EmojiType.Suprised);
        emojiPlayer.StopRepeat();
        if (intoWater)
        {
            PlayListSound(0, true);
        }
        else
        {
            PlayListSound(5, false);
        }
    }

    public void PlayListSound(int index, bool isAuto)
    {
        emojiPlayer.ChangeClips(audioClipsCollection[index].audioClips);
        emojiPlayer.SetRandom(false);
        if (isAuto)
        {
            emojiPlayer.PlaySoundAuto();
        }
        else
        {
            emojiPlayer.PlaySoundManual();
        }
    }

    public void SetJumpEmoji()
    {
        emojiPlayer.StopRepeat();
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
            emojiPlayer.StopRepeat();
            emojiPlayer.ChangeClips(audioClipsCollection[6].audioClips);
            emojiPlayer.SetRandom(false);
            emojiPlayer.PlaySoundManual();
            SetEmojiSprite(EmojiType.Disgusted);
        }
        else
        {
            emojiPlayer.StopRepeat();
            emojiPlayer.ChangeClips(audioClipsCollection[4].audioClips);
            emojiPlayer.SetRandom(false);
            emojiPlayer.PlaySoundManual();
            SetEmojiSprite(EmojiType.Puked);
        }
    }

    public void StartFastSpining(bool tf)
    {
        if (inFastSpining == tf) { return; }
        inFastSpining = tf;
        fastSpinTimer = 0.0f;
    }

    public void FallBackDown()
    {
        Invoke(nameof(FallBackEmoji), GameManager.instance.GameScriptObj.BarrelEmojiFallBackDelay);
    }

    public void FallBackEmoji()
    {
        if (Mathf.Abs(GameManager.instance.GameCon.barrelHighestY - transform.position.y) > GameManager.instance.GameScriptObj.BarrelEmojiFallBackThreshold)
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
            emojiPlayer.StopRepeat();
            emojiPlayer.ChangeClips(audioClipsCollection[2].audioClips);
            emojiPlayer.SetRandom(false);
            emojiPlayer.PlaySoundManual();
            inFastSpining = false;
            isEmojiTypeCoolDowned = false;
            Invoke(nameof(EmojiChangeOffCoolDown), GameManager.instance.GameScriptObj.BarrelEmojiOffFallCoolDownTime);
        }
        GameManager.instance.GameCon.barrelHighestY = transform.position.y;
    }

    public void EmojiChangeOffCoolDown()
    {
        isEmojiTypeCoolDowned = true;
    }

    public void SetClimbing()
    {
        if (isClimbing) { return;}
        emojiPlayer.StopRepeat();
        emojiPlayer.ChangeClips(audioClipsCollection[3].audioClips);
        emojiPlayer.SetRandom(false);
        emojiPlayer.PlaySoundAuto();
        isClimbing = true;
        SetEmojiSprite(EmojiType.Sorry);
    }

    public void SetSmashWall()
    {
        SetEmojiSprite(EmojiType.Unsettle);
        emojiPlayer.StopRepeat();
        emojiPlayer.ChangeClips(audioClipsCollection[1].audioClips);
        emojiPlayer.SetRandom(true);
        emojiPlayer.PlaySoundManual();
        Invoke(nameof(SetInjured), GameManager.instance.GameScriptObj.BarrelEmojiInjuredTime);
    }

    public void SetInjured()
    {
        emojiPlayer.StopRepeat();
        SetEmojiSprite(EmojiType.Injured);
    }

    public void SetStand()
    {
        emojiPlayer.StopRepeat();
        SetEmojiSprite(EmojiType.Force);
    }

    public void SetLook()
    {
        emojiPlayer.StopRepeat();
        SetEmojiSprite(EmojiType.OhWhat);
    }

    public void SetNormal()
    {
        if (inWater)
        {
            SetEmojiSprite(EmojiType.Dead);
            PlayListSound(0, true);
            return;
        }

        emojiPlayer.StopRepeat();

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
        if (!isEmojiTypeCoolDowned) { return; }
        if (type != EmojiType.Sorry)
        {
            isClimbing = false;
        }
        CancelInvoke(nameof(SetNormal));
        Invoke(nameof(SetNormal), inWater ? GameManager.instance.GameScriptObj.BarrelEmojiBackNormalInWaterTime : GameManager.instance.GameScriptObj.BarrelEmojiBackNormalTime);
        emojiRend.sprite = GameManager.instance.GameScriptObj.EmojiTypes[(int)type];
    }
}
