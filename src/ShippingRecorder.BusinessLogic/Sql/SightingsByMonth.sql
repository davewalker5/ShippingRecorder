SELECT  STRFTIME('%Y', s.Date ) AS "Year",
        STRFTIME('%m', s.Date ) AS "Month",
        COUNT( DISTINCT s.Id ) AS "Sightings",
        COUNT( DISTINCT l.Id ) AS "Locations",
        COUNT( DISTINCT v.Id ) AS "Vessels"
FROM SIGHTING s
INNER JOIN LOCATION l ON l.Id = s.Location_Id
INNER JOIN VESSEL v ON v.Id = s.Vessel_Id
WHERE s.Date BETWEEN '$from' AND '$to'
GROUP BY STRFTIME('%Y', s.Date ), STRFTIME('%m', s.Date )
ORDER BY s.Date ASC;
