import React, { Component } from 'react';
import {getShortenedLink} from "../api";
import "./Home.scss";

export class Home extends Component {
  static displayName = Home.name;
  state = {
      link: '', 
      sending: false,
      shortenedLink: null,
      error: null
  }
    
  async generate () {
    if (this.state.sending)
        return;
    try {
        this.setState({error: null})
        const linkId = await getShortenedLink(this.state.link);
        this.setState({
            shortenedLink: location.origin + '/' + linkId,
            sending: false
        })
    } catch (e) {
        this.setState({
            sending: false,
            error: e.toString()
        })
    }
  }
  
  render () {
    return (
      <div className="is-flex is-flex-direction-column is-align-items-center home">
          <h1 className="app-title">Shorty</h1>
          <input className="url-input"
              onChange={e => this.setState({link: e.target.value})} 
              type="text"/>
          <button className="button is-primary" onClick={() => this.generate()}>Shortify</button>
          {this.state.shortenedLink ?
              <a href={this.state.shortenedLink} target="_blank">{this.state.shortenedLink}</a> : undefined}
      </div>
    );
  }
}
