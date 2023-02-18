﻿using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPITrainingCreateModel
{
    [Transaction(TransactionMode.Manual)]
    public class Main : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            List<Wall> walls = CreateWalls(doc, 10000, 5000, GetLevel(doc, "Уровень 1"), GetLevel(doc, "Уровень 2"));

            return Result.Succeeded;
        }

        public Level GetLevel(Document doc, string inputString)
        {
            List<Level> listLevel = new FilteredElementCollector(doc)
                                    .OfClass(typeof(Level))
                                    .OfType<Level>()
                                    .ToList();

            Level level = listLevel
               .Where(x => x.Name.Equals(inputString))
               .FirstOrDefault();

            return level;
        }

        public List<Wall> CreateWalls(Document doc, double widthInput, double depthInput, Level level1, Level level2)
        {

            double width = UnitUtils.ConvertToInternalUnits(widthInput, UnitTypeId.Millimeters);
            double depth = UnitUtils.ConvertToInternalUnits(depthInput, UnitTypeId.Millimeters);
            double dx = width / 2;
            double dy = depth / 2;

            List<XYZ> points = new List<XYZ>();
            points.Add(new XYZ(-dx, -dy, 0));
            points.Add(new XYZ(dx, -dy, 0));
            points.Add(new XYZ(dx, dy, 0));
            points.Add(new XYZ(-dx, dy, 0));
            points.Add(new XYZ(-dx, -dy, 0));

            List<Wall> walls = new List<Wall>();

            Transaction transaction = new Transaction(doc, "Построение стен");
            transaction.Start();
            for (int i = 0; i < 4; i++)
            {
                Line line = Line.CreateBound(points[i], points[i + 1]);
                Wall wall = Wall.Create(doc, line, level1.Id, false);
                walls.Add(wall);
                wall.get_Parameter(BuiltInParameter.WALL_HEIGHT_TYPE).Set(level2.Id);
            }
            transaction.Commit();

            return walls;
        }

    }
}
