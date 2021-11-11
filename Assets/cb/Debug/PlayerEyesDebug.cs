using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEditor.Animations;
using UnityEngine;

class PlayerEyesDebug : MonoBehaviour
{
    public GameObject ContainerPrototype;
    public TMP_Text LabelPrototype;
    public TMP_Text ValuePrototype;

    public TMP_Text HitText;
    public TMP_Text ColliderText;

    public TMP_Text PalletText;
    public TMP_Text PickupText;
    public TMP_Text PanelText;
    public TMP_Text BlockText;
    public TMP_Text BuildingBlockText;
    public TMP_Text InteractableText;

    public PlayerEyes Eyes;

    void Update()
    {
        Set(HitText, Eyes.HitObject);
        Set(ColliderText, Eyes.CollidedObject);

        Set(PalletText, Eyes.PalletInSights);
        Set(PickupText, Eyes.PickupInSights);
        Set(PanelText, Eyes.PanelInSights);
        Set(BlockText, Eyes.BlockInSights);
        Set(BuildingBlockText, Eyes.BuildingBlockInSights);
        Set(InteractableText, Eyes.InteractableInSights);
    }

    void Set(TMP_Text text, GameObject obj)
    {
        if (obj == null)
            text.text = "---";
        else
            text.text = obj.name;
    }

    void Set(TMP_Text text, MonoBehaviour obj)
    {
        if (obj == null)
            text.text = "---";
        else
            text.text = obj.name;
    }

    void Reset()
    {
        if (LabelPrototype == null || ValuePrototype == null)
        {
            var toDestroy = new List<GameObject>();

            GameObject prototypes = null;

            foreach (Transform child in transform)
                if (child.gameObject.name == "prototypes")
                    prototypes = child.gameObject;
                else
                    toDestroy.Add(child.gameObject);

            foreach (var obj in toDestroy)
                DestroyImmediate(obj);

            if (prototypes == null)
                return;

            foreach (Transform child in prototypes.transform)
            {
                if (child.gameObject.name == "label")
                    LabelPrototype = child.gameObject.GetComponent<TMP_Text>();
                else if (child.gameObject.name == "value")
                    ValuePrototype = child.gameObject.GetComponent<TMP_Text>();
                else if (child.gameObject.name == "container")
                    ContainerPrototype = child.gameObject;
            }

            if (LabelPrototype == null || ValuePrototype == null)
                return;
        }

        HitText = Setup("Hit");
        ColliderText = Setup("Collider");

        PalletText = Setup("Pallet");
        PickupText = Setup("Pickup");
        PanelText = Setup("Panel");
        BlockText = Setup("Block");
        BuildingBlockText = Setup("Building Block");
        InteractableText = Setup("Interactable");
    }

    TMP_Text Setup(string label)
    {
        var container = Instantiate(ContainerPrototype, transform);
        container.name = label;

        var lbl = Instantiate(LabelPrototype, container.transform);
        lbl.name = label + " label";

        var val = Instantiate(ValuePrototype, container.transform);
        val.name = label + " value";

        lbl.text = label + ":";
        val.text = "---";

        return val;
    }
}