using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputTest : MonoBehaviour
{
    public GameObject P1DisplayText, P2DisplayText, P3DisplayText, P4DisplayText;

    private List<string> p1buttonsText, p2buttonsText, p3buttonsText, p4buttonsText;

    //private bool                           fire, horiz, vert, jump, slide, alth, altv, gsw, cancel
    private bool[] p1buttons = new bool[9] { false, false,false,false,false,false,false,false,false};
    private bool[] p2buttons = new bool[9] { false, false,false,false,false,false,false,false,false};
    private bool[] p3buttons = new bool[9] { false, false,false,false,false,false,false,false,false};
    private bool[] p4buttons = new bool[9] { false, false,false,false,false,false,false,false,false};

    // Start is called before the first frame update
    void Start()
    {
        p1buttonsText = new List<string>();
        p2buttonsText = new List<string>();
        p3buttonsText = new List<string>();
        p4buttonsText = new List<string>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        P1Check();
        P2Check();
        P3Check();
        P4Check();
    }

    private void P1Check()
    {
        if (Input.GetAxis("P1Fire1") != 0 && !p1buttons[0])
        {
            p1buttonsText.Add("\nFire1");

            p1buttons[0] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p1buttonsText)
            {
                if (s == "\nFire1") iRemove = p1buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p1buttonsText.Remove(p1buttonsText[iRemove]);

            p1buttons[0] = false;
        }

        if (Input.GetAxis("P1Horizontal") != 0 && !p1buttons[1])
        {
            p1buttonsText.Add("\nHorizontal");

            p1buttons[1] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p1buttonsText)
            {
                if (s == "\nP1Horizontal") iRemove = p1buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p1buttonsText.Remove(p1buttonsText[iRemove]);

            p1buttons[1] = false;
        }

        if (Input.GetAxis("P1Vertical") != 0 && !p1buttons[2])
        {
            p1buttonsText.Add("\nVertical");

            p1buttons[2] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p1buttonsText)
            {
                if (s == "\nP1Vertical") iRemove = p1buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p1buttonsText.Remove(p1buttonsText[iRemove]);

            p1buttons[2] = false;
        }

        if (Input.GetButton("P1Jump") && !p1buttons[3])
        {
            p1buttonsText.Add("\nJump");

            p1buttons[3] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p1buttonsText)
            {
                if (s == "\nJump") iRemove = p1buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p1buttonsText.Remove(p1buttonsText[iRemove]);

            p1buttons[3] = false;
        }

        if (Input.GetButton("P1Slide") && !p1buttons[4])
        {
            p1buttonsText.Add("\nSlide");

            p1buttons[4] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p1buttonsText)
            {
                if (s == "\nSlide") iRemove = p1buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p1buttonsText.Remove(p1buttonsText[iRemove]);

            p1buttons[4] = false;
        }

        if (Input.GetAxis("P1 Alt Horizontal") != 0 && !p1buttons[5])
        {
            p1buttonsText.Add("\nAlt Horiz");

            p1buttons[5] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p1buttonsText)
            {
                if (s == "\nAlt Horiz") iRemove = p1buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p1buttonsText.Remove(p1buttonsText[iRemove]);

            p1buttons[5] = false;
        }

        if (Input.GetAxis("P1 Alt Vertical") != 0 && !p1buttons[6])
        {
            p1buttonsText.Add("\nAlt Vert");

            p1buttons[6] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p1buttonsText)
            {
                if (s == "\nAlt Vert") iRemove = p1buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p1buttonsText.Remove(p1buttonsText[iRemove]);

            p1buttons[6] = false;
        }

        if (Input.GetButton("P1GrappleSwitch") && !p1buttons[7])
        {
            p1buttonsText.Add("\nGrapple Switch");

            p1buttons[7] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p1buttonsText)
            {
                if (s == "\nGrapple Switch") iRemove = p1buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p1buttonsText.Remove(p1buttonsText[iRemove]);

            p1buttons[7] = false;
        }

        if (Input.GetButton("P1Cancel") && !p1buttons[8])
        {
            p1buttonsText.Add("\nCancel");

            p1buttons[8] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p1buttonsText)
            {
                if (s == "\nCancel") iRemove = p1buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p1buttonsText.Remove(p1buttonsText[iRemove]);

            p1buttons[8] = false;
        }

        P1DisplayText.GetComponent<Text>().text = "";
        foreach (string s in p1buttonsText) P1DisplayText.GetComponent<Text>().text += s;
    }

    private void P2Check()
    {
        if (Input.GetAxis("P2Fire1") != 0 && !p2buttons[0])
        {
            p2buttonsText.Add("\nFire1");

            p2buttons[0] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p2buttonsText)
            {
                if (s == "\nFire1") iRemove = p2buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p2buttonsText.Remove(p2buttonsText[iRemove]);

            p2buttons[0] = false;
        }

        if (Input.GetAxis("P2Horizontal") != 0 && !p2buttons[1])
        {
            p1buttonsText.Add("\nHorizontal");

            p2buttons[1] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p2buttonsText)
            {
                if (s == "\nHorizontal") iRemove = p2buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p2buttonsText.Remove(p2buttonsText[iRemove]);

            p2buttons[1] = false;
        }

        if (Input.GetAxis("P2Vertical") != 0 && !p2buttons[2])
        {
            p2buttonsText.Add("\nVertical");

            p2buttons[2] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p2buttonsText)
            {
                if (s == "\nVertical") iRemove = p2buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p2buttonsText.Remove(p2buttonsText[iRemove]);

            p2buttons[2] = false;
        }

        if (Input.GetButton("P2Jump") && !p2buttons[3])
        {
            p2buttonsText.Add("\nJump");

            p2buttons[3] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p2buttonsText)
            {
                if (s == "\nJump") iRemove = p2buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p2buttonsText.Remove(p2buttonsText[iRemove]);

            p2buttons[3] = false;
        }

        if (Input.GetButton("P2Slide") && !p2buttons[4])
        {
            p2buttonsText.Add("\nSlide");

            p2buttons[4] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p2buttonsText)
            {
                if (s == "\nSlide") iRemove = p2buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p2buttonsText.Remove(p2buttonsText[iRemove]);

            p2buttons[4] = false;
        }

        if (Input.GetAxis("P2 Alt Horizontal") != 0 && !p2buttons[5])
        {
            p2buttonsText.Add("\nAlt Horiz");

            p2buttons[5] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p2buttonsText)
            {
                if (s == "\nAlt Horiz") iRemove = p2buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p2buttonsText.Remove(p2buttonsText[iRemove]);

            p2buttons[5] = false;
        }

        if (Input.GetAxis("P2 Alt Vertical") != 0 && !p2buttons[6])
        {
            p2buttonsText.Add("\nAlt Vert");

            p2buttons[6] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p2buttonsText)
            {
                if (s == "\nAlt Vert") iRemove = p2buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p2buttonsText.Remove(p2buttonsText[iRemove]);

            p2buttons[6] = false;
        }

        if (Input.GetButton("P2GrappleSwitch") && !p2buttons[7])
        {
            p2buttonsText.Add("\nGrapple Switch");

            p2buttons[7] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p2buttonsText)
            {
                if (s == "\nGrapple Switch") iRemove = p2buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p2buttonsText.Remove(p2buttonsText[iRemove]);

            p2buttons[7] = false;
        }

        if (Input.GetButton("P2Cancel") && !p2buttons[8])
        {
            p2buttonsText.Add("\nCancel");

            p2buttons[8] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p2buttonsText)
            {
                if (s == "\nCancel") iRemove = p2buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p2buttonsText.Remove(p2buttonsText[iRemove]);

            p2buttons[8] = false;
        }

        P2DisplayText.GetComponent<Text>().text = "";
        foreach (string s in p2buttonsText) P2DisplayText.GetComponent<Text>().text += s;
    }

    private void P3Check()
    {
        if (Input.GetAxis("P3Fire1") != 0 && !p3buttons[0])
        {
            p3buttonsText.Add("\nFire1");

            p3buttons[0] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p3buttonsText)
            {
                if (s == "\nFire1") iRemove = p3buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p3buttonsText.Remove(p3buttonsText[iRemove]);

            p3buttons[0] = false;
        }

        if (Input.GetAxis("P3Horizontal") != 0 && !p3buttons[1])
        {
            p3buttonsText.Add("\nHorizontal");

            p3buttons[1] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p3buttonsText)
            {
                if (s == "\nHorizontal") iRemove = p3buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p3buttonsText.Remove(p3buttonsText[iRemove]);

            p3buttons[1] = false;
        }

        if (Input.GetAxis("P3Vertical") != 0 && !p3buttons[2])
        {
            p3buttonsText.Add("\nVertical");

            p3buttons[2] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p3buttonsText)
            {
                if (s == "\nVertical") iRemove = p3buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p3buttonsText.Remove(p3buttonsText[iRemove]);

            p3buttons[2] = false;
        }

        if (Input.GetButton("P3Jump") && !p3buttons[3])
        {
            p3buttonsText.Add("\nJump");

            p3buttons[3] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p3buttonsText)
            {
                if (s == "\nJump") iRemove = p3buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p3buttonsText.Remove(p3buttonsText[iRemove]);

            p3buttons[3] = false;
        }

        if (Input.GetButton("P3Slide") && !p3buttons[4])
        {
            p3buttonsText.Add("\nSlide");

            p3buttons[4] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p3buttonsText)
            {
                if (s == "\nSlide") iRemove = p3buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p3buttonsText.Remove(p3buttonsText[iRemove]);

            p3buttons[4] = false;
        }

        if (Input.GetAxis("P3 Alt Horizontal") != 0 && !p3buttons[5])
        {
            p3buttonsText.Add("\nAlt Horiz");

            p3buttons[5] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p3buttonsText)
            {
                if (s == "\nAlt Horiz") iRemove = p3buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p3buttonsText.Remove(p3buttonsText[iRemove]);

            p3buttons[5] = false;
        }

        if (Input.GetAxis("P3 Alt Vertical") != 0 && !p3buttons[6])
        {
            p3buttonsText.Add("\nAlt Vert");

            p3buttons[6] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p3buttonsText)
            {
                if (s == "\nAlt Vert") iRemove = p3buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p3buttonsText.Remove(p3buttonsText[iRemove]);

            p3buttons[6] = false;
        }

        if (Input.GetButton("P3GrappleSwitch") && !p3buttons[7])
        {
            p3buttonsText.Add("\nGrapple Switch");

            p3buttons[7] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p3buttonsText)
            {
                if (s == "\nGrapple Switch") iRemove = p3buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p3buttonsText.Remove(p3buttonsText[iRemove]);

            p3buttons[7] = false;
        }

        if (Input.GetButton("P3Cancel") && !p3buttons[8])
        {
            p3buttonsText.Add("\nCancel");

            p3buttons[8] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p3buttonsText)
            {
                if (s == "\nCancel") iRemove = p3buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p3buttonsText.Remove(p3buttonsText[iRemove]);

            p3buttons[8] = false;
        }

        P3DisplayText.GetComponent<Text>().text = "";
        foreach (string s in p3buttonsText) P3DisplayText.GetComponent<Text>().text += s;
    }

    private void P4Check()
    {
        if (Input.GetAxis("P4Fire1") != 0 && !p4buttons[0])
        {
            p4buttonsText.Add("\nFire1");

            p4buttons[0] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p4buttonsText)
            {
                if (s == "\nFire1") iRemove = p4buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p4buttonsText.Remove(p4buttonsText[iRemove]);

            p4buttons[0] = false;
        }

        if (Input.GetAxis("P4Horizontal") != 0 && !p4buttons[1])
        {
            p4buttonsText.Add("\nHorizontal");

            p4buttons[1] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p4buttonsText)
            {
                if (s == "\nHorizontal") iRemove = p4buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p4buttonsText.Remove(p4buttonsText[iRemove]);

            p4buttons[1] = false;
        }

        if (Input.GetAxis("P4Vertical") != 0 && !p4buttons[2])
        {
            p4buttonsText.Add("\nVertical");

            p4buttons[2] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p4buttonsText)
            {
                if (s == "\nVertical") iRemove = p4buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p4buttonsText.Remove(p4buttonsText[iRemove]);

            p4buttons[2] = false;
        }

        if (Input.GetButton("P4Jump") && !p4buttons[3])
        {
            p4buttonsText.Add("\nJump");

            p4buttons[3] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p4buttonsText)
            {
                if (s == "\nJump") iRemove = p4buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p4buttonsText.Remove(p4buttonsText[iRemove]);

            p4buttons[3] = false;
        }

        if (Input.GetButton("P4Slide") && !p4buttons[4])
        {
            p4buttonsText.Add("\nSlide");

            p4buttons[4] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p4buttonsText)
            {
                if (s == "\nSlide") iRemove = p4buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p4buttonsText.Remove(p4buttonsText[iRemove]);

            p4buttons[4] = false;
        }

        if (Input.GetAxis("P4 Alt Horizontal") != 0 && !p4buttons[5])
        {
            p4buttonsText.Add("\nAlt Horiz");

            p4buttons[5] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p4buttonsText)
            {
                if (s == "\nAlt Horiz") iRemove = p4buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p4buttonsText.Remove(p4buttonsText[iRemove]);

            p4buttons[5] = false;
        }

        if (Input.GetAxis("P4 Alt Vertical") != 0 && !p4buttons[6])
        {
            p4buttonsText.Add("\nAlt Vert");

            p4buttons[6] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p4buttonsText)
            {
                if (s == "\nAlt Vert") iRemove = p4buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p4buttonsText.Remove(p4buttonsText[iRemove]);

            p4buttons[6] = false;
        }

        if (Input.GetButton("P4GrappleSwitch") && !p4buttons[7])
        {
            p4buttonsText.Add("\nGrapple Switch");

            p4buttons[7] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p4buttonsText)
            {
                if (s == "\nGrapple Switch") iRemove = p4buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p4buttonsText.Remove(p4buttonsText[iRemove]);

            p4buttons[7] = false;
        }

        if (Input.GetButton("P4Cancel") && !p4buttons[8])
        {
            p4buttonsText.Add("\nCancel");

            p4buttons[8] = true;
        }
        else
        {
            int iRemove = -1;

            foreach (string s in p4buttonsText)
            {
                if (s == "\nCancel") iRemove = p4buttonsText.IndexOf(s);
            }

            if (iRemove != -1) p4buttonsText.Remove(p4buttonsText[iRemove]);

            p4buttons[8] = false;
        }

        P4DisplayText.GetComponent<Text>().text = "";
        foreach (string s in p4buttonsText) P4DisplayText.GetComponent<Text>().text += s;
    }
}
