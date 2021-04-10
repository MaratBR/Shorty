import React, { Component } from 'react';
import {ApiError, getShortenedLink} from "../api";
import "./Home.scss";

const PLACEHOLDERS = [
    'yandex.ru',
    'google.com',
    'youtube.com',
    'facebook.com',
    'youtube.com/watch?v=dQw4w9WgXcQ'
];

export class Home extends Component {
  static displayName = Home.name;
  state = {
      link: '',
      loading: false,
      errors: null
  }
    
  async generate () {
    if (this.state.loading)
        return;
    try {
        const linkId = await getShortenedLink(this.state.link);
        this.setState({
            link: location.origin + '/' + linkId,
            loading: false,
            error: null
        })
    } catch (e) {
        let error = e.toString();
        if (e instanceof ApiError) {
            error = e.errors;
        }
        this.setState({
            loading: false,
            error
        })
    }
  }
  
  render () {
    return (
      <div className="is-flex is-flex-direction-column is-align-items-center home">
          <h1 className="app-title">Shorty</h1>
          <div>
              <input
                  disabled={this.state.loading}
                  onFocus={e => e.target.select()}
                  value={this.state.link}
                  placeholder={PLACEHOLDERS[Math.floor(Math.random() * PLACEHOLDERS.length)]}
                  className="url-input"
                  onChange={e => this.setState({link: e.target.value})}
                  type="text"/>
          </div>
          {this.state.error ? this.renderErrors(this.state.error) : undefined}
          <button
              disabled={this.state.link.trim() === ''}
              className={"button is-primary" + (this.state.loading ? ' is-loading ' : '')}
              onClick={() => this.generate()}>Shortify</button>
          
      </div>
    );
  }

  renderErrors(errors) {
      const items = [];
      
      let index = 0;
      for (let field in errors) {
          for (let err of errors[field]) {
              items.push(
                  <div className="error" key={index}>
                      <span className="error__field">{field}</span>
                      <span className="error__body">{err}</span>
                  </div>
              );
              index++;
          }
      }
      
      return <div className="errors">{items}</div>
  }
}
