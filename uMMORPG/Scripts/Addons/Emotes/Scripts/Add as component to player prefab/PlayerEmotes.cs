using Mirror;
using System.Collections;
using UnityEngine;

// PLAYER EMOTES

public class PlayerEmotes : NetworkBehaviour
{
    public Player plyer;

    [Header("[-=-[ Emotes Configuration ]-=-]")]
    public Tmpl_Emotes configurationEmotes;
    private bool emoteUp = false;

    // -----------------------------------------------------------------------------------
    // Update
    // -----------------------------------------------------------------------------------
    private void Update() //TODO il faudrait plutot creer une UI qui permette de cliquer sur les emotes
    {
        Player player = Player.localPlayer;
        if (!player) return;

        if (emoteUp == false && player.isAlive && !UIUtils.AnyInputActive() && (player.state == "IDLE" || player.state == "MOVING") )
        {
            CheckEmotes();
        }
    }

    public override void OnStartClient()
    {
        plyer.chat.onSubmit.SetListener(EmoteCommand);
    }


    private void EmoteCommand(string textSend)
    {
        int i = 0;

        foreach (EmotesAnimation item in configurationEmotes.animations)
        {
            if (textSend == item.chatCommand)
            {
                ShowAnimation(i, true);
                break;
            }
            ++i;
        }

        int i2 = 0;

        foreach (EmotesEmoji item in configurationEmotes.emotes)
        {
            if (textSend == item.chatCommand)
            {
                ShowEmote(i2);
                break;
            }
            ++i;
        }
    }

    // -----------------------------------------------------------------------------------
    // emoteWait
    // -----------------------------------------------------------------------------------
    private IEnumerator emoteWait(float fWaitTime)
    {
        yield return new WaitForSeconds(fWaitTime);
        emoteUp = false;
    }

    // -----------------------------------------------------------------------------------
    // ShowEmote
    // @Client
    // -----------------------------------------------------------------------------------
    [Client]
    private void ShowEmote(int index)
    {
        Player player = Player.localPlayer;
        if (!player && !configurationEmotes) return;

        Cmd_ShowEmote(index);
        emoteUp = true;

        if (configurationEmotes.emotes[index].soundEffect != null && player.audioSource != null)
            player.audioSource.PlayOneShot(configurationEmotes.emotes[index].soundEffect);

        IEnumerator coroutine = emoteWait(configurationEmotes.secondsBetweenEmotes);
        StartCoroutine(coroutine);
    }

    // -----------------------------------------------------------------------------------
    // Cmd_ShowEmote
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    private void Cmd_ShowEmote(int nIndex)
    {
        Player player = GetComponent<Player>();
        if (!player) return;
        Rpc_ShowEmote(nIndex);
    }

    [ClientRpc]
    private void Rpc_ShowEmote(int nIndex)
    {
        Player player = GetComponent<Player>();
        if (configurationEmotes.emotes.Length >= nIndex && configurationEmotes.emotes[nIndex].emote != null)
        {
            GameObject emoteObject = null;
            emoteObject = Instantiate(configurationEmotes.emotes[nIndex].emote, transform.position + configurationEmotes.emotes[nIndex].distanceAboveHead, Quaternion.identity) as GameObject;

            if (emoteObject)
            {
                emoteObject.transform.parent = player.gameObject.transform;
            }
        }
    }


    // -----------------------------------------------------------------------------------
    // ShowAnimation
    // @Client
    // -----------------------------------------------------------------------------------
    [Client]
    private void ShowAnimation(int nIndex, bool bStart)
    {
        Player player = Player.localPlayer;
        if (!player) return;

        Cmd_ShowAnimation(nIndex, bStart);
        emoteUp = true;

        float fWaitTime = configurationEmotes.secondsBetweenEmotes;
        if (configurationEmotes.animations[nIndex].secondsBetweenEmotes > 0)
            fWaitTime = configurationEmotes.animations[nIndex].secondsBetweenEmotes;

        if (bStart && configurationEmotes.animations[nIndex].soundEffect != null && player.audioSource != null)
            player.audioSource.PlayOneShot(configurationEmotes.animations[nIndex].soundEffect);

        IEnumerator coroutine = emoteWait(fWaitTime);
        StartCoroutine(coroutine);
    }

    // -----------------------------------------------------------------------------------
    // Cmd_ShowAnimation
    // @Client -> @Server
    // -----------------------------------------------------------------------------------
    [Command]
    private void Cmd_ShowAnimation(int nIndex, bool start)
    {
        Player player = GetComponent<Player>();
        if (!player) return;

        if (configurationEmotes.animations.Length >= nIndex && !string.IsNullOrWhiteSpace(configurationEmotes.animations[nIndex].animationName))
        {
            Rpc_ShowAnimatione(nIndex, start);
            /*if (start)
                player.animator.Play(configurationEmotes.animations[nIndex].animationName);
            else
                player.StopAnimation(configurationEmotes.animations[nIndex].animationName);*/
        }
    }

    [ClientRpc]
    private void Rpc_ShowAnimatione(int nIndex, bool start)
    {
        Player player = GetComponent<Player>();
        if (!player) return;

        if (configurationEmotes.animations.Length >= nIndex && !string.IsNullOrWhiteSpace(configurationEmotes.animations[nIndex].animationName))
        {
            if (start)
                player.animator.Play(configurationEmotes.animations[nIndex].animationName);
            else
                player.StopAnimation(configurationEmotes.animations[nIndex].animationName);
        }
    }

    // -----------------------------------------------------------------------------------
    // checkEmotes
    // @Client
    // -----------------------------------------------------------------------------------
    [ClientCallback]
    private void CheckEmotes()
    {
        for (int i = 0; i < configurationEmotes.emotes.Length; ++i)
        {
            if (configurationEmotes.emotes[i].emote == null) return;

            if (Input.GetKey(configurationEmotes.emotesHotKey) && Input.GetKeyDown(configurationEmotes.emotes[i].hotKey))
                ShowEmote(i);

            
    		if (Input.GetKey(configurationEmotes.emotesHotKey) && !Input.GetKey(configurationEmotes.animationsHotKey) && Input.GetKeyDown(configurationEmotes.emotes[i].hotKey))
    			ShowEmote(i);
    		
        }
        
    	for (int i = 0; i < configurationEmotes.animations.Length; ++i) {
    		if (string.IsNullOrWhiteSpace(configurationEmotes.animations[i].animationName)) return;

    		if (Input.GetKey(configurationEmotes.animationsHotKey) && !Input.GetKey(configurationEmotes.emotesHotKey) && Input.GetKeyDown(configurationEmotes.animations[i].hotKey))
    			ShowAnimation(i, true);

    		if (Input.GetKey(configurationEmotes.animationsHotKey) && !Input.GetKey(configurationEmotes.emotesHotKey) && Input.GetKeyUp(configurationEmotes.animations[i].hotKey))
    			ShowAnimation(i, false);
    	}
    	
    }

    // -----------------------------------------------------------------------------------
}