using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Day1 : MonoBehaviour
{
    private int objectsInteracted = 0;


   public void updateInteraction(){
objectsInteracted++;
if (objectsInteracted ==3){
FindObjectOfType<SceneController>().LoadNextScene();
   }}
}
