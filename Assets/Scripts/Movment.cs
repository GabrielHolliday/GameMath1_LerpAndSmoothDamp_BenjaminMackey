using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEditorInternal;
using UnityEngine;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using Unity.VisualScripting;
using Unity.Mathematics;


public class NewMonoBehaviourScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject cube1;
    public GameObject cube2;
    static bool move = true;

    static CancellationTokenSource source;
    
  

    enum Easing
    {
        none,
        sineIAssume
    }

    static async void CubeAnim(Easing easing, GameObject obj, int intervalsInMs, CancellationToken token)
    {

        Vector3 targUp = obj.transform.position + new Vector3(0, 5, 0);
        Vector3 targDown = obj.transform.position + new Vector3(0, -5, 0);
        Vector3 targNext;
        Vector3 iniPos = obj.transform.position;
        bool FlipFlop = false;
        while (move)
        {
            if (FlipFlop) targNext = targDown;
            else targNext = targUp;
            FlipFlop = !FlipFlop;

            Vector3 nextPos;
            Vector3 speed = new Vector3(0, 0, 0);
            for (int i = 0; i < intervalsInMs; i++)
            {

                if (easing == Easing.sineIAssume)
                {
                    nextPos = Vector3.SmoothDamp(obj.transform.position, targNext, ref speed, 0.02f, 80);
                    speed = nextPos - obj.transform.position;
                }
                else
                {
                    nextPos = Vector3.Lerp(iniPos, targNext, (float)i/ (float)intervalsInMs);
                }

                if (token.IsCancellationRequested)
                {
                    UnityEngine.Debug.Log("Canceling...");
                    return;
                }

                obj.transform.position = nextPos;
                await Task.Delay(1);
            }
            

            iniPos = obj.transform.position;
        }

    }
    
    
    static void CancelMove(PlayModeStateChange state)
    {
        if(state == PlayModeStateChange.ExitingPlayMode)
        {
            source.Cancel();
            source.Dispose();
        }
    }

    
    void Start()
    {
        source = new CancellationTokenSource();
        EditorApplication.playModeStateChanged += CancelMove;
        CubeAnim(Easing.sineIAssume, cube1, 1000, source.Token);
        CubeAnim(Easing.none, cube2, 1000, source.Token);
        
    }
    

    // Update is called once per frame
    void Update()
    {
        if(!EditorApplication.isPlaying)
        {
            move = false;
        }
    
    }
}
