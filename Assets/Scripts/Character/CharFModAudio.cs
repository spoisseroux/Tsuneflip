using UnityEngine;

public class CharFModAudio : MonoBehaviour
{

    private FMOD.Studio.EventInstance footstep;
    private FMOD.Studio.EventInstance otherfootstep;
    private FMOD.Studio.EventInstance jump;
    private FMOD.Studio.EventInstance land;

    private void PlayFootstep()
    {
        //footstep = FMODUnity.RuntimeManager.CreateInstance("event:/ClickA");
        footstep = FMODUnity.RuntimeManager.CreateInstance("event:/PlayFootstep");
        footstep.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        footstep.start();
        footstep.release();
    }

    private void PlayOtherFootstep()
    {
        //footstep = FMODUnity.RuntimeManager.CreateInstance("event:/ClickA");
        otherfootstep = FMODUnity.RuntimeManager.CreateInstance("event:/PlayOtherFootstep");
        otherfootstep.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        otherfootstep.start();
        otherfootstep.release();
    }

    private void PlayJump()
    {
        //jump = FMODUnity.RuntimeManager.CreateInstance("event:/ArrowUp");
        jump = FMODUnity.RuntimeManager.CreateInstance("event:/PlayJump");
        jump.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        jump.start();
        jump.release();
    }

    private void PlayLand()
    {
        //land = FMODUnity.RuntimeManager.CreateInstance("event:/PlayLand");
        //land.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        //land.start();
        //land.release();
    }
}