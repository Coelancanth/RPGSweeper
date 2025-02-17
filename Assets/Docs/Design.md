# 机制
我的扫雷RPG游戏有下面的机制： 

- 踩到了雷并不是必死，而是根据数值计算来判断，比如玩家有20HP，一个雷的数值是10，踩到这颗雷后，玩家的HP=20-10=10，但是也获得了对应的经验值，经验值在得到后可以升级，升级后HP增加，同时恢复全满。
- 数字的提示如普通扫雷一样，但实时计算（在触发雷后，重新计算数值并更新）
- 「空白」方块作为提示，（比如森林方块，可能提示附近有xxx的类型）
- 不同的雷有不同的作用效果（无需触发）
	- 将周围的数字改为？
	- 可以伪装自己（本身其实是怪物，但样子与宝箱一样） 
- 不同的雷有不同的触发效果（消灭后触发），比如： 
	- 可以恢复所有的HP（生命卷轴） 
	- 可以获得经验值（宝箱） 
	- 可以将某种雷变为另一种雷 
	- 可以多次触发（在多次触发后，雷才算是被消灭） 
	- 可以显示周围一定范围（包括空白） 
	- 可以生成物品，物品可以拖曳，有不同的功能
- 特定的生成模板
	- 特殊的绝对位置（比如棋盘的边缘）
	- 特殊的相对位置（某种雷被某种其他的雷所包围）
	

- Passive Effects: 
	- Persistent, fundamental conditions
	- They are applied and persist until removed
- Active Effects: 
	- One-time triggers
	- 
- State: 
	- Temporary conditions set by active effects 
	- With clear enter/ exit points