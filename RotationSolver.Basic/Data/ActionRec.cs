﻿namespace RotationSolver.Basic.Data;
using Action = Lumina.Excel.GeneratedSheets.Action;

public record ActionRec(DateTime UsedTime, Action Action);

public record DamageRec(DateTime ReceiveTime, float Ratio);
