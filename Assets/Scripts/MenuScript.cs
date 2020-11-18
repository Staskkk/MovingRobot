using System;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public Toggle[] travelToggles;
    public TextMeshProUGUI stopResumeButtonText;
    public TMP_InputField[] pointsInputs;
    public TMP_InputField startXInput;
    public TMP_InputField endXInput;
    public TMP_InputField stepInput;
    public TMP_InputField robotSpeedInput;
    public TMP_InputField funcCodeInput;
    public GameObject menuPanel;

    public RobotMoveScript robotMoveScript;
    public ChartControlScript chartControlScript;

    private readonly CultureInfo Culture = CultureInfo.InvariantCulture;

    public enum TravelType
    {
        Points = 0,
        Curve = 1,
        Lagrange = 2,
        Spline = 3
    }

    public TravelType travelType = TravelType.Points;

    private void Start()
    {
        robotMoveScript.movingFinished += RobotMoveScriptOnMovingFinished;
    }

    private void RobotMoveScriptOnMovingFinished(object sender, EventArgs e)
    {
        ToggleMenu();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        menuPanel.SetActive(!menuPanel.activeSelf);
    }

    public void ToggleOnChecked(string travelTypeStr)
    {
        var travelTypeEnum = (TravelType) Enum.Parse(typeof(TravelType), travelTypeStr);
        int currentIndex = (int)travelTypeEnum;
        if (!travelToggles[currentIndex].isOn)
        {
            return;
        }

        for (int i = 0; i < travelToggles.Length; ++i)
        {
            if (i != currentIndex)
            {
                travelToggles[i].isOn = false;
            }
        }

        travelType = travelTypeEnum;
    }

    public void StartButtonOnClick()
    {
        string[] pointsX = pointsInputs[0].text.Split(new string[] { "\n", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        string[] pointsY = pointsInputs[1].text.Split(new string[] { "\n", Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        Vector2[] points = new Vector2[pointsX.Length];
        for (int i = 0; i < pointsX.Length; ++i)
        {
            points[i] = new Vector2(float.Parse(pointsX[i], Culture), float.Parse(pointsY[i], Culture));
        }

        float step = float.Parse(stepInput.text, Culture);
        ITravel travel = null;
        switch (travelType)
        {
            case TravelType.Points:
                travel = new PointsTravel(points);
                break;
            case TravelType.Curve:
                travel = new CurveTravel(funcCodeInput.text, float.Parse(startXInput.text, Culture), float.Parse(endXInput.text, Culture), step);
                break;
            case TravelType.Lagrange:
                travel = new LagrangeTravel(points, step);
                break;
            case TravelType.Spline:
                travel = new SplineTravel(points, step);
                break;
        }

        chartControlScript.ClearPoints();
        if (travelType != TravelType.Curve)
        {
            chartControlScript.GeneratePoints(points);
        }

        robotMoveScript.moveSpeed = float.Parse(robotSpeedInput.text, Culture);
        robotMoveScript.travel = travel;
        robotMoveScript.StartMovement();
        stopResumeButtonText.text = "Остановить";
        ToggleMenu();
    }

    public void StopResumeButtonOnClick()
    {
        if (robotMoveScript.isMoving)
        {
            robotMoveScript.StopMovement();
            stopResumeButtonText.text = "Возобновить";
        }
        else
        {
            robotMoveScript.ResumeMovement();
            stopResumeButtonText.text = "Остановить";
            ToggleMenu();
        }
    }
}
