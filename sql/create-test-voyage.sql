SELECT * FROM PORT WHERE Code IN ( 'GBSOU', 'BEZEE' );
/*
6901	22	BEZEE	Zeebrugge
59315	235	GBSOU	Southampton
*/
SELECT * FROM OPERATOR WHERE Name LIKE 'P&O%';
/*
310	P&O Cruises
311	P&O Cruises Australia
*/
SELECT v.*
FROM VESSEL v
INNER JOIN REGISTRATION_HISTORY rh ON rh.Vessel_Id = v.Id
WHERE rh.Name = 'Arcadia';
-- 3	9226906	2005	8.2	285	33
INSERT INTO VOYAGE ( Operator_Id, Vessel_ID, Number ) VALUES ( 310, 3, 'I413' );
SELECT * FROM VOYAGE WHERE Number = 'I413';
-- 1	I413	310	3
INSERT INTO VOYAGE_EVENT ( Voyage_Id, Event_Type, Port_Id, Date ) VALUES ( 1, 0, 59315, '2024-05-14 00:00:00' );
INSERT INTO VOYAGE_EVENT ( Voyage_Id, Event_Type, Port_Id, Date ) VALUES ( 1, 1, 6901, '2024-05-15 00:00:00' );
INSERT INTO VOYAGE_EVENT ( Voyage_Id, Event_Type, Port_Id, Date ) VALUES ( 1, 0, 6901, '2024-05-15 00:00:00' );
INSERT INTO VOYAGE_EVENT ( Voyage_Id, Event_Type, Port_Id, Date ) VALUES ( 1, 1, 59315, '2024-05-17 00:00:00' );
