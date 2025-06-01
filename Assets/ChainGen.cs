using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class ChainGen : MonoBehaviour
{
    public Slider animTimeBar;
    public GameObject testObj_,infoPanel,playPanel,lastSpring,chainPref;
    public AnimationClip saveClip;
    public Animation _animation;
    //private GameObjectRecorder ;
    private bool doRecord = false;
    public bool playBackMode,randomForce;
    public Transform cameraPlayer;
    private SpringJoint lastSpringJoint;
    public Toggle pullingSp,consForce;
    public float chainHeight, stackForce, loadRateSp,forceMaxSp,BrownMulti, relTorq;

    private static Vector3 camPosSTATIC;
    private static Quaternion camRotSTATIC;
    public static int nRingSTATIC=20;
    public int nRing;
    public Transform initialLink;
    public List<Transform> metals;
    static public float stackForceSTATIC;
    private ConstantForce forceTipCons;
    public TMP_InputField dragInput,pullRateInput, forceTip_Input, nRingsInput, brownianInput, stackForceInput;
    public List<Rigidbody> rings;
    public Button resetButton,releaseButton, recButton, playButton;
    public Material mat1, mat2, mat3;
    public System.Random rng;
    private void Start()
    {
        rng = new System.Random();
        //UpdateParams();

        nRing = nRingSTATIC;
        nRingsInput.text = nRingSTATIC.ToString();
        metals = new List<Transform>(); metals.Add(chainPref.transform.Find("metal1").transform); metals.Add(chainPref.transform.Find("metal2").transform);


        pullingSp.onValueChanged.AddListener(delegate { consForce.isOn = false;lastSpring.transform.position = rings.Last().position; loadRateSp = -float.Parse(pullRateInput.text); lastSpring.SetActive(pullingSp.isOn);  });
        consForce.onValueChanged.AddListener(delegate { pullingSp.isOn = false; lastSpring.SetActive(false);ChangeTipForce(-float.Parse(forceTip_Input.text));  });
        forceTip_Input.onEndEdit.AddListener(delegate { float ii=Mathf.Max(-200,-float.Parse(forceTip_Input.text)); forceTip_Input.text = (-ii).ToString();ChangeTipForce(ii); });
        stackForceInput.onEndEdit.AddListener(delegate { stackForce = float.Parse(stackForceInput.text); SwitchStacks(); });
        dragInput.onEndEdit.AddListener(delegate { ChangeDrags(float.Parse(dragInput.text)); });

        pullRateInput.onEndEdit.AddListener(delegate { loadRateSp=Mathf.Max(-float.Parse(pullRateInput.text),-200); pullRateInput.text = (-loadRateSp).ToString(); });
        brownianInput.onEndEdit.AddListener(delegate { BrownMulti = float.Parse(brownianInput.text); if (BrownMulti == 0) { randomForce = false; } else { randomForce = true; } });
        nRingsInput.onEndEdit.AddListener(delegate { nRingSTATIC = int.Parse(nRingsInput.text); });
        resetButton.onClick.AddListener(delegate { ResetScene(); });
        releaseButton.onClick.AddListener(delegate { ReleaseMetals(); });

        animTimeBar.onValueChanged.AddListener(delegate { SetAnimTime(); });



        for (int i = 0; i < nRing; i++)
        {
            GameObject GO = Instantiate(chainPref, new Vector3(initialLink.position.x, initialLink.position.y- (i+1)* (chainHeight), initialLink.position.z), Quaternion.Euler(chainPref.transform.localRotation.x +90, chainPref.transform.localRotation.y + 90 * ((i+1) % 2), chainPref.transform.localRotation.z), transform);
            GO.name = chainPref.name + "_" + i;
            GO.transform.Find("Satel1").GetComponent<SatelScript>().linkNum = i;
            GO.transform.Find("Satel2").GetComponent<SatelScript>().linkNum = i;

            if (i % 2 == 0) { GO.GetComponentsInChildren<MeshRenderer>()[2].sharedMaterial =mat2; }
            metals.Add(GO.transform.Find("metal1").transform);
            metals.Add(GO.transform.Find("metal2").transform);
            rings.Add(GO.GetComponent<Rigidbody>());
            if (i== nRing - 1)
            {
                forceTipCons = GO.GetComponent<ConstantForce>();
                forceTipCons.force = new Vector3(0f,-2f,0f);
                lastSpring.transform.position = rings.Last().position;
                lastSpringJoint = lastSpring.GetComponent<SpringJoint>();
                lastSpringJoint.connectedBody = rings.Last();
            }
            
        }

        rings.Last().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;


        //GOR = new GameObjectRecorder(gameObject);
        //GOR.BindComponentsOfType<Transform>(gameObject, true);
    }

    public void TogglePlayBackMode()
    {
        playBackMode = !playBackMode;
        playPanel.SetActive(playBackMode);
        Camera.main.backgroundColor = playBackMode ? Color.darkGray : Color.white;
        foreach (Rigidbody rb in rings)
        {
            rb.isKinematic = playBackMode;
            _animation.Stop();
            //_animation.enabled = false;
        }        
        if (playBackMode)
        {
            GetComponent<Animation>().Play("Anim1");
        }

    }


    private void LateUpdate()
    {
        if (playBackMode)
        {
            animTimeBar.SetValueWithoutNotify((_animation["Anim1"].normalizedTime % 1));
        }
    }
    public void SetAnimTime()
    {
        if (!playBackMode) return;
        _animation["Anim1"].normalizedTime = animTimeBar.value;
    }

    public void TogglePlayAnimationCall()
    {
        if (!playBackMode) TogglePlayBackMode();
        playButton.GetComponent<Image>().color = _animation["Anim1"].speed != 0 ? Color.white : Color.lightCoral;
        if (_animation["Anim1"].speed != 0)
            PauseAnimationCall();
        else
            PlayAnimationCall();
    }
    public void ClosePlayPanel()
    {
        PauseAnimationCall();
        TogglePlayBackMode();
    }
    public void PauseAnimationCall()
    {
        if (!playBackMode) return;
        _animation["Anim1"].speed = 0;
    }
    public void PlayAnimationCall()
    {
        if (!playBackMode) TogglePlayBackMode();
        _animation["Anim1"].enabled = true;
        _animation.Play("Anim1");
        _animation["Anim1"].speed = 1;
    }
    public void EnableObj(int eventNum)
    {
        if (!playBackMode) return;

        switch (eventNum)
        {
            case 0:
                testObj_.SetActive(true);
                PauseAnimationCall();
                break;
        }
    }

    public void ToggleInfo()
    {
       infoPanel.SetActive(!infoPanel.activeSelf);
    }


    private void Update()
    {
        stackForceSTATIC = stackForce;

        if (UnityEngine.Input.GetKeyDown(KeyCode.R))
            //ToggleRecord();

        if (doRecord)
        {
            //GOR.TakeSnapshot(Time.deltaTime);
        }

    }
    private void FixedUpdate()
    {
        if (randomForce) ApplyRandomForce();
   

        if (pullingSp.isOn && !playBackMode)
        {
            loadRateSp = ((lastSpringJoint.currentForce.y > forceMaxSp) && loadRateSp <= 0f)?0f:-float.Parse(pullRateInput.text);
            lastSpring.transform.position += new Vector3(0f, loadRateSp*0.0001f, 0f);
        }
    }

    private void ResetScene()
    {
        camPosSTATIC = cameraPlayer.position;
        camRotSTATIC = cameraPlayer.rotation;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        cameraPlayer.position = camPosSTATIC;
        cameraPlayer.rotation = camRotSTATIC;
    }
    public void ToggleRecord()
    {
        if (playBackMode) return;
        doRecord = !doRecord;
        recButton.GetComponentInChildren<TextMeshProUGUI>().text = doRecord ? "Stop recording (R)": "Record (R)";
        recButton.GetComponent<Image>().color = doRecord ? Color.royalBlue:Color.white;

        if (!doRecord)
        {
            AnimationClip newClip = new AnimationClip();
            newClip.name = "Anim1";
            //GOR.SaveToClip(saveClip);
            saveClip.wrapMode = WrapMode.Loop;
            //UnityEditor.AssetDatabase.CreateAsset(newClip, UnityEditor.AssetDatabase.GenerateUniqueAssetPath("Assets/Anim1.anim"));
            _animation.AddClip(newClip, newClip.name);
            playButton.gameObject.SetActive(true);
        }
    }
    private void ReleaseMetals()
    {
        for (int i = 0; i < metals.Count; i++)
        {
            metals[i].parent=null;
            metals[i].gameObject.GetComponent<SphereCollider>().enabled = false;
            Rigidbody metu=metals[i].gameObject.AddComponent<Rigidbody>();
            metu.useGravity = false;
            metu.AddForce(new Vector3((float)rng.NextDouble() - .5f, (float)rng.NextDouble() - .5f, (float)rng.NextDouble() - .5f) * 5f, ForceMode.Impulse);
        }
    }

    private void ChangeTipForce(float tipForceValue)
    {
        forceTipCons.force = new Vector3(0f, consForce.isOn?tipForceValue:0f, 0f);
    }


    private void ChangeDrags(float dr)
    {
        for (int i = 0; i < rings.Count; i++)
        {
            rings[i].linearDamping = dr * 0.08f;
            rings[i].angularDamping = dr * 0.08f;
        }
    }

    public void SwitchStacks()
    {
        chainPref.transform.Find("SatelColl1").gameObject.SetActive(stackForce > 0);
        chainPref.transform.Find("SatelColl2").gameObject.SetActive(stackForce > 0);
        chainPref.transform.Find("Satel1").gameObject.SetActive(stackForce > 0);
        chainPref.transform.Find("Satel2").gameObject.SetActive(stackForce > 0);
        for (int i = 0; i < rings.Count; i++)
        {
            rings[i].transform.Find("SatelColl1").gameObject.SetActive(stackForce > 0);
            rings[i].transform.Find("SatelColl2").gameObject.SetActive(stackForce > 0);
            rings[i].transform.Find("Satel1").gameObject.SetActive(stackForce > 0);
            rings[i].transform.Find("Satel2").gameObject.SetActive(stackForce > 0);
        }
    }
    public void CloseApp()
    {
        Application.Quit();
    }
    private void ApplyRandomForce()
    {
        for (int i = 0; i < rings.Count; i++)
        {
            Unity.Mathematics.float3 F, T;
            Brownian(rng, out F, out T, BrownMulti / 100f, BrownMulti * relTorq / 100f);
            rings[i].AddForce(F.x, F.y, F.z, ForceMode.Impulse);
            rings[i].AddTorque(T.x, T.y, T.z, ForceMode.Impulse);
        }
    }
    public void Brownian(System.Random rng, out Unity.Mathematics.float3 F, out Unity.Mathematics.float3 T, float fMult, float tMult)
    {
        double u1=0f, u2=0f, s=0f;
        F = new();
        T = new();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 10 && (s >= 1.0 || s == 0); j++)
            {
                u1 = rng.NextDouble()*2-1f;
                u2 = rng.NextDouble()*2-1f;
                s = u1 * u1 + u2 * u2;
            } 
            double factor = Math.Sqrt(-2.0 * Math.Log(s) / s);
            F[i] = (float)(fMult* u1 * factor);
            T[i] = (float)(tMult* u2 * factor);
        }
    }
}
