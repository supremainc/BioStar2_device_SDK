#pragma once

#include <vector>
#include <string>
#include "BS_API.h"


class DoorControl
{
public:
	DoorControl(void* sdkContext);
	virtual ~DoorControl();


public:
	int getAllDoor(BS2_DEVICE_ID id, std::vector<BS2Door>& doors);
	int setDoor(BS2_DEVICE_ID id, std::vector<BS2Door>& doors);
	int removeAllDoor(BS2_DEVICE_ID id);
	int lockDoor(BS2_DEVICE_ID id);
	int unlockDoor(BS2_DEVICE_ID id, std::vector<BS2_DOOR_ID>& doorIDs);

	void print(const BS2Door& door);
	void print(const BS2DoorRelay& relay);
	void print(const BS2DoorSensor& sensor);
	void print(const BS2ExitButton& button);
	void print(const BS2Action& action);
	void print(const BS2RelayAction& relay);
	void print(const BS2OutputPortAction& relay);
	void print(const BS2DisplayAction& relay);
	void print(const BS2SoundAction& relay);
	void print(const BS2LedAction& relay);
	void print(const BS2BuzzerAction& relay);
	void print(const BS2LiftAction& relay);
	void print(const BS2Signal& signal);


private:
	static void* context_;
};
