; Generated G-code
; Printer Model: Ender 3
; Filament Type: ABS
; Nozzle Size: 0.4
; Layer Height: 2
; Extruder Temperature: 2
; Bed Temperature: 2
; Retraction: 2
; Print Speed: 2
; Bed Leveling: Yes (G29)
; Cooling Speed: 2
G21 ; Set units to millimeters
G90 ; Use absolute positioning
M104 S2 ; Set extruder temperature to 2°C
M140 S2 ; Set bed temperature to 2°C
M190 S2 ; Wait for bed to reach temperature
M107 ; Turn off fan
G1 Z2 ; Set initial layer height
G1 X112 Y150 Z0 E0.25132741228718347 ; Move to start point and extrude
G1 X112 Y150 Z0 E0.25132741228718347 ; Draw line to end point and extrude
G1 X112 Y150 Z0 E0.25132741228718347 ; Move to start point and extrude
G1 X290 Y219 Z0 E0.25132741228718347 ; Draw line to end point and extrude
G1 X290 Y219 Z0 E0.25132741228718347 ; Move to start point and extrude
G1 X290 Y219 Z0 E0.25132741228718347 ; Draw line to end point and extrude
G1 F2 ; Set print speed
M104 S0 ; Turn off extruder
M140 S0 ; Turn off bed
M106 S0 ; Turn off fan
G28 X0 Y0 ; Move to home position
M84 ; Disable motors
