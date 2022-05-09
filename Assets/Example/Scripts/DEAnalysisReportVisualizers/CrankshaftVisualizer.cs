using dExplorer.Runtime.Mathematics;
using Unity.Mathematics;
using UnityEngine;

public class CrankshaftVisualizer : DEAnalysisReportVisualizer<Float2DEAnalysisReport, Float2DEAnalysisValue, Vector2>
{
    #region Fields
    public float CrankRadius = 1.5f;
    public float ConnectingRodLength = 3.0f;
    public float PistonRadius = 0.5f;
    public float NonCompressibleLength = 0.5f;
	#endregion Fields

	#region Methods
	private new void OnValidate()
	{
        SolvingType = DESolvingType.EXPLICIT_RUNGE_KUTTA_4;
        ParameterStepIndex = 0;
        base.OnValidate();
	}

	private void OnDrawGizmos()
	{
        float angularPosition = Variable.y;

        Vector3 crankRodConnectionPosition = new Vector3(
            CrankRadius * math.sin(angularPosition),
            CrankRadius * math.cos(angularPosition),
            0.0f);

        Vector3 rodPistonConnectionPosition = new Vector3(
            0.0f,
            crankRodConnectionPosition.y + math.sqrt(ConnectingRodLength * ConnectingRodLength - crankRodConnectionPosition.x * crankRodConnectionPosition.x),
            0.0f);

        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.DrawLine(Vector3.zero, crankRodConnectionPosition);
        Gizmos.DrawLine(crankRodConnectionPosition, rodPistonConnectionPosition);

        Gizmos.DrawLine(rodPistonConnectionPosition + Vector3.left * PistonRadius, rodPistonConnectionPosition + Vector3.right * PistonRadius);
        Gizmos.DrawLine(rodPistonConnectionPosition + Vector3.back * PistonRadius, rodPistonConnectionPosition + Vector3.forward * PistonRadius);

        Gizmos.DrawLine(new Vector3(-PistonRadius, ConnectingRodLength - CrankRadius, 0.0f), new Vector3(PistonRadius, ConnectingRodLength - CrankRadius, 0.0f));
        Gizmos.DrawLine(new Vector3(0.0f, ConnectingRodLength - CrankRadius, -PistonRadius), new Vector3(0.0f, ConnectingRodLength - CrankRadius, PistonRadius));

        Gizmos.DrawLine(new Vector3(-PistonRadius, ConnectingRodLength + CrankRadius, 0.0f), new Vector3(PistonRadius, ConnectingRodLength + CrankRadius, 0.0f));
        Gizmos.DrawLine(new Vector3(0.0f, ConnectingRodLength + CrankRadius, -PistonRadius), new Vector3(0.0f, ConnectingRodLength + CrankRadius, PistonRadius));

        Gizmos.DrawLine(new Vector3(-PistonRadius, ConnectingRodLength + CrankRadius + NonCompressibleLength, 0.0f), new Vector3(PistonRadius, ConnectingRodLength + CrankRadius + NonCompressibleLength, 0.0f));
        Gizmos.DrawLine(new Vector3(0.0f, ConnectingRodLength + CrankRadius + NonCompressibleLength, -PistonRadius), new Vector3(0.0f, ConnectingRodLength + CrankRadius + NonCompressibleLength, PistonRadius));

        Gizmos.DrawSphere(Vector3.zero, CrankRadius * 0.1f);
        Gizmos.DrawSphere(crankRodConnectionPosition, CrankRadius * 0.1f);
        Gizmos.DrawSphere(rodPistonConnectionPosition, CrankRadius * 0.1f);
    }
	#endregion Methods
}
