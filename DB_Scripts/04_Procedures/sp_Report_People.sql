/****** Object:  StoredProcedure [dbo].[sp_Report_People]    Script Date: 8/18/2025 11:34:32 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE OR ALTER PROCEDURE [dbo].[sp_Report_People]
AS
BEGIN 
    WITH cte AS
    (
        SELECT 
            P.PersonID, 
            P.Person, 
            S.[Name],
            ISNULL(V.VLA_Location, '') AS VLA,
			ISNULL(V.AHVLA, '') AS APHALocation, -- This is the APHA location
            CASE 
                WHEN EXISTS (SELECT 1 FROM [dbo].[tTraining] T WHERE T.TrainerID = P.PersonID) THEN 'Yes'
                ELSE 'No'
            END AS Trainer,
            CASE 
                WHEN EXISTS (SELECT 1 FROM [dbo].[tTraining] T WHERE T.PersonID = P.PersonID) THEN 'Yes'
                ELSE 'No'
            END AS Trainee,
            CASE 
                WHEN EXISTS (SELECT 1 FROM [dbo].[tTraining] T WHERE T.PersonID = P.PersonID) THEN 'Yes'
                ELSE 'No'
            END AS Trained
        FROM [dbo].[tPerson] P
        INNER JOIN [dbo].[tSites] S ON S.PlantNo = P.LocationID
        LEFT JOIN [dbo].[tblVLALoc] V ON LTRIM(RTRIM(V.Loc_ID)) = LTRIM(RTRIM(CAST(P.VLALocationID AS NVARCHAR(50))))
    )
    SELECT * FROM cte ORDER BY cte.[Name]
END

--ALTER TABLE tblTrainee
--  ALTER COLUMN Trainee
--    NVARCHAR(100) COLLATE Latin1_General_CI_AS  NULL
