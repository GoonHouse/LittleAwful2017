using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class LoadData : MonoBehaviour {

    private SaveDatum sd = null;

    public GameObject ui_music_volume = null;
    public GameObject ui_effects_volume = null;
    public GameObject ui_flashing_lights = null;
    public GameObject ui_ircNick = null;
    public GameObject ui_ircPass = null;
    public GameObject ui_ircChannel = null;
    public GameObject ui_ircServer = null;
    public GameObject ui_ircPort = null;

    // Use this for initialization
    void Start () {

        sd = God.main.GetComponent<SaveData>().loadedSave;
        sd.playerUse [0] = 2; // Set player 1 to human

        ui_music_volume.GetComponent<Slider>().normalizedValue = sd.musicVolume;
        ui_effects_volume.GetComponent<Slider>().normalizedValue = sd.effectsVolume;
        ui_flashing_lights.GetComponent<Slider>().normalizedValue = sd.flashingLights;

        ui_ircNick.GetComponentInParent<InputField>().text = sd.ircNick;
        ui_ircNick.GetComponentInParent<InputField>().onValueChanged.AddListener(delegate { ircNickChangeCheck(); });
        ui_ircPass.GetComponentInParent<InputField>().text = sd.ircPass;
        ui_ircPass.GetComponentInParent<InputField>().onValueChanged.AddListener(delegate { ircPassChangeCheck(); });
        ui_ircChannel.GetComponentInParent<InputField>().text = sd.ircChannel;
        ui_ircChannel.GetComponentInParent<InputField>().onValueChanged.AddListener(delegate { ircChannelChangeCheck(); });
        ui_ircServer.GetComponentInParent<InputField>().text = sd.ircServer;
        ui_ircServer.GetComponentInParent<InputField>().onValueChanged.AddListener(delegate { ircServerChangeCheck(); });
        ui_ircPort.GetComponentInParent<InputField>().text = sd.ircPort.ToString();
        ui_ircPort.GetComponentInParent<InputField>().onValueChanged.AddListener(delegate { ircPortChangeCheck(); });
    }

    public void uiMusicVolumeChangeCheck() {
        sd.musicVolume = ui_music_volume.GetComponent<Slider>().normalizedValue;
    }

    public void uiEffectsVolumeChangeCheck() {
        sd.effectsVolume = ui_effects_volume.GetComponent<Slider>().normalizedValue;
    }

    public void uiFlashingLightsCheck() {
        sd.flashingLights = ui_flashing_lights.GetComponent<Slider>().normalizedValue;
    }

    public void ircNickChangeCheck() {
        sd.ircNick = ui_ircNick.GetComponentInParent<InputField>().text;
    }

    public void ircPassChangeCheck() {
        sd.ircPass = ui_ircPass.GetComponentInParent<InputField>().text;
    }

    public void ircChannelChangeCheck() {
        sd.ircChannel= ui_ircChannel.GetComponentInParent<InputField>().text;
    }

    public void ircServerChangeCheck() {
        sd.ircServer = ui_ircServer.GetComponentInParent<InputField>().text;
    }

    public void ircPortChangeCheck() {
        var default_port = 6667;
        var port = God.StrToIntDef(ui_ircPort.GetComponentInParent<InputField>().text, default_port);
        sd.ircPort = port >= 1 && port <= 65535 ? port : default_port;
        ui_ircPort.GetComponentInParent<InputField>().text = port.ToString();
    }

    public void player1UseOptionsCheck(int i) {
        sd.playerUse[0] = i;
    }
    public void player2UseOptionsCheck(int i) {
        sd.playerUse[1] = i;
    }
    public void player3UseOptionsCheck(int i) {
        sd.playerUse[2] = i;
    }
    public void player4UseOptionsCheck(int i) {
        sd.playerUse[3] = i;
    }

}
