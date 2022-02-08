pragma solidity ^0.4.18;

contract runner{

    struct Player {
	uint user_index;
	uint high_score; // high score remove
	uint[3] item_list;
	uint reward;
	bool success;
    }
    Player[] public players;

    // item list 
    uint[3] public init_item_list = [0,0,0];
    uint[3] public item_price = [100,200,300]; // add item price to calculate buy function

    function getPlayer_high_score(uint _index) public view returns (uint){
	return players[_index].high_score;
    }
    function getPlayer_item_list(uint _index) public view returns (uint[3]){
	return players[_index].item_list;
    }
    function getPlayer_reward(uint _index) public view returns (uint){ 
	return players[_index].reward;
    }
    function getPlayer_length() public view returns (uint){ 
	return players.length;
    }
    function isRecentTradeSuccess(uint _index) public view returns (bool){ 
	return players[_index].success;
    }

    function createPlayer(uint _index) public{
	players.push(Player(players.length, 0, init_item_list, 0, false));
    }

    function updatePlayer_info(uint _index, uint _current_score, uint[3] _used_itemlist) public {
	players[_index].item_list[0] = _used_itemlist[0];
	players[_index].item_list[1] = _used_itemlist[1];
	players[_index].item_list[2] = _used_itemlist[2];

	if (players[_index].high_score < _current_score){
	    players[_index].high_score = _current_score;
	}
	players[_index].reward += _current_score * 2;
    }

    function send(uint fromUserIndex, uint toUserIndex, uint itemIndex, uint itemCount) public{
	if (players[fromUserIndex].item_list[itemIndex] > itemCount) {
	    players[fromUserIndex].item_list[itemIndex] -= itemCount;
	    players[toUserIndex].item_list[itemIndex] += itemCount;

	    players[fromUserIndex].success = true;
	    players[toUserIndex].success = true;
	} else{
	    players[fromUserIndex].success = false;
	    players[toUserIndex].success = false;
	}
    }

    //data is translated, but function has view keyword.
    //and it is different from send or buy form.
    //deprecated now
    //function trade(uint _first_UserIndex, uint[3] _fist_itemCount, uint _second_UserIndex, uint[3] _second_itemCount) public view returns (bool){
    //    for(uint i=0; i<3; i++)
    //     {
    //        if(players[_first_UserIndex].item_list[i] < _fist_itemCount[i]){
    //            return false;
    //        }
    //        if(players[_second_UserIndex].item_list[i] < _second_itemCount[i]){
    //            return false;
    //        }
	    //item trade
    //        players[_first_UserIndex].item_list[i] =+ _second_itemCount[i];
    //        players[_second_UserIndex].item_list[i] =+ _fist_itemCount[i];
    //        return true;
    //     }
    //}

    function buy(uint _index, uint itemIndex, uint itemCount) public{
	uint total_cost;
	total_cost = itemCount * item_price[itemIndex];

	if(players[_index].reward > total_cost){
	    players[_index].reward -= total_cost;
	    players[_index].item_list[itemIndex] += itemCount;
	    players[_index].success = true;
	} else{
	    players[_index].success = false;
	}
    }
}
